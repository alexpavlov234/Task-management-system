using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Popups;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Shared;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages.Projects
{
    public partial class ProjectModal
    {
        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }
        [Inject]
        private IProjectService ProjectService { get; set; }
        [Inject]
        private BaseHelper BaseHelper { get; set; }
        [Inject]
        private IKeyValueService keyValueService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IIssueService IssueService { get; set; }
        private readonly bool IsUserNew = false;
        private bool IsVisible = false;
        private readonly DateTime MinDate = new DateTime(1900, 1, 1);
        private ToastMsg toast = new ToastMsg();
        private readonly SfDialog sfDialog = new SfDialog();
        private bool updateParticipants = false;
        private bool isLoggedUserAdmin = false;
        private EditContext editContext;
        private Project project = new Project();
        private Project originalProject;
        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }
        private ApplicationUser loggedUser { get; set; }
        private SfMultiSelect<ApplicationUser[], ApplicationUser> projectParticipantsSfMultiSelect { get; set; } = new SfMultiSelect<ApplicationUser[], ApplicationUser>();
        [System.Text.Json.Serialization.JsonIgnore]
        private ApplicationUser[] projectParticipants { get; set; }
        public async void OpenDialog(Project project)
        {
            originalProject = ProjectService.GetProjectById(project.ProjectId);
            projectTypes = keyValueService.GetAllKeyValuesByKeyType("ProjectType");
            users = UserService.GetAllUsers();
            projectParticipants = null;
            loggedUser = UserService.GetLoggedUser();
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            if (project.ProjectId == 0)
            {
                project.ProjectType = projectTypes.First().Name;
                if (!isLoggedUserAdmin)
                {
                    project.ProjectOwner = loggedUser;
                }
            }
            this.project = new Project() { ProjectId = project.ProjectId, ProjectParticipants = project.ProjectParticipants, Issues = project.Issues, ProjectOwner = project.ProjectOwner, ProjectType = project.ProjectType, EndDate = project.EndDate, ProjectDescription = project.ProjectDescription, ProjectName = project.ProjectName, StartDate = project.StartDate };
            projectParticipantsSfMultiSelect.DataSource = users;
            if (this.project.ProjectParticipants != null)
            {
                projectParticipants = this.project.ProjectParticipants.Select(x => x.User).Where(x => x != null).ToArray();
            }
            else
            {
                projectParticipants = new ApplicationUser[] { };
            }
            editContext = new EditContext(this.project);
            IsVisible = true;
            StateHasChanged();
        }
        private void OnValueSelectHandlerParticipants(MultiSelectChangeEventArgs<ApplicationUser[]> args)
        {
            if (projectParticipants != null)
            {
                IEnumerable<ApplicationUserProject> participants = projectParticipants.Select(x =>
                    new ApplicationUserProject
                    {
                        UserId = x.Id,
                        ProjectId = project.ProjectId
                    });
                project.ProjectParticipants = participants.ToList();
            }
            else
            {
                project.ProjectParticipants = null;
            }
        }
        private void CloseDialog()
        {
            IsVisible = false;
            StateHasChanged();
        }
        private void UpdateDialog()
        {
            projectTypes = keyValueService.GetAllKeyValuesByKeyType("ProjectType");
            users = UserService.GetAllUsers();
            projectParticipantsSfMultiSelect.DataSource = users;
            if (project.ProjectParticipants != null)
            {
                //TODO:Да го фикснеш;
                // this.project.ProjectParticipants.Remove(x => x.);
                projectParticipants = project.ProjectParticipants.Select(x => x.User).Where(x => x != null).ToArray();
            }
            else
            {
                projectParticipants = new ApplicationUser[] { };
            }
            editContext = new EditContext(project);
            StateHasChanged();
        }
        private async void SaveProject()
        {
            if (editContext.Validate())
            {
                if (!(project.ProjectId == 0))
                {
                    if (updateParticipants)
                    {
                        string result = ProjectService.UpdateProject(project);
                        await CallbackAfterSubmit.InvokeAsync();
                        if (result.StartsWith("Успешно"))
                        {
                            toast.sfSuccessToast.Title = result;
                            _ = toast.sfSuccessToast.ShowAsync();
                            UpdateDialog();
                        }
                        else
                        {
                            toast.sfErrorToast.Title = result;
                            _ = toast.sfErrorToast.ShowAsync();
                        }
                        UpdateDialog();
                    }
                    else
                    {
                        project.ProjectParticipants = originalProject.ProjectParticipants;
                        UpdateDialog();
                    }
                }
                else
                {
                    string result = ProjectService.CreateProject(project);
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        _ = toast.sfSuccessToast.ShowAsync();
                        UpdateDialog();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        _ = toast.sfErrorToast.ShowAsync();
                    }
                    await CallbackAfterSubmit.InvokeAsync();
                }
            }
            else
            {
                toast.sfErrorToast.Title = editContext.GetValidationMessages().FirstOrDefault();
                _ = toast.sfErrorToast.ShowAsync();
            }
        }
        private void ValidateParticipants()
        {
            if (project.ProjectParticipants == null)
            {
                toast.sfErrorToast.Title = "Моля въведете участници в проекта!";
                _ = toast.sfErrorToast.ShowAsync();
            }
            else if (project.ProjectParticipants.Count() == 0)
            {
                toast.sfErrorToast.Title = "Моля въведете участници в проекта!";
                _ = toast.sfErrorToast.ShowAsync();
            }
            else
            {
                if (originalProject != null)
                {
                    foreach (ApplicationUserProject participant in originalProject.ProjectParticipants)
                    {
                        if (IssueService.GetAllIssuesByProjectAndApplicationUser(project.ProjectId, participant.UserId).Any() && !project.ProjectParticipants.Where(x => x.UserId == participant.UserId).Any())
                        {
                            toast.sfErrorToast.Title = "Опитвате се да премахнете потребители, които имат възложени задачи! Моля, първо изтрийте задачите!";
                            updateParticipants = false;
                            _ = toast.sfErrorToast.ShowAsync();
                            return;
                        }
                    }
                }
                updateParticipants = true;
            }
        }
    }
}