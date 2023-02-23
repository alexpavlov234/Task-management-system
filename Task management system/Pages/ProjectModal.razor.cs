using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Popups;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Common;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages
{
    public partial class ProjectModal
    {
        protected EditContext editContext;
        private Project project = new Project();
        private bool IsUserNew = false;
        private bool IsVisible = false;
        private DateTime MinDate = new DateTime(1900, 1, 1);
        private ToastMsg toast = new ToastMsg();
        private SfDialog sfDialog = new SfDialog();

        bool isLoggedUserAdmin = false;
        private ApplicationUser loggedUser { get; set; }
        private SfMultiSelect<ApplicationUser[], ApplicationUser> projectParticipantsSfMultiSelect { get; set; } =
            new SfMultiSelect<ApplicationUser[], ApplicationUser>();
        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        private ApplicationUser[] projectParticipants { get; set; }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        private BaseHelper BaseHelper { get; set; }

        [Inject]
        private IKeyValueService keyValueService { get; set; }

        [Inject]
        private IUserService UserService { get; set; }


        protected async override Task OnInitializedAsync()
        {
            projectTypes = keyValueService.GetAllKeyValuesByKeyType("ProjectType");
            users = UserService.GetAllUsers();
        }
        public async void OpenDialog(Project project)

        {
            projectParticipants = null;
            loggedUser = UserService.GetLoggedUser();
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            if (!isLoggedUserAdmin)
            {
                project.ProjectOwner = loggedUser;
            }
            
            this.project = new Project() { ProjectId = project.ProjectId, ProjectParticipants = project.ProjectParticipants, Issues = project.Issues, ProjectOwner = project.ProjectOwner, ProjectTypeId = project.ProjectTypeId, EndDate = project.EndDate, ProjectDescription = project.ProjectDescription, ProjectName = project.ProjectName, ProjectType = project.ProjectType, StartDate = project.StartDate };
            projectTypes = keyValueService.GetAllKeyValuesByKeyType("ProjectType");
            users = UserService.GetAllUsers();
            projectParticipantsSfMultiSelect.DataSource = users;
            if (this.project.ProjectParticipants != null)
            {
                //TODO:Да го фикснеш;
                // this.project.ProjectParticipants.Remove(x => x.);
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

            //this.projectParticipants.DistinctBy(i => i.User);
            //TODO: Да се обмисли
            if (projectParticipants == null)
            {
                projectParticipants = new ApplicationUser[] { };
            }

            IEnumerable<ApplicationUserProject> participants = projectParticipants.Select(x => new ApplicationUserProject
            {
                UserId = x.Id,
                ProjectId = project.ProjectId
            });


            project.ProjectParticipants = participants.ToList();


            // project.ProjectParticipants = new List<ApplicationUser>(projectParticipants);
            if (editContext.Validate())

            {

                if (!(project.ProjectId == 0))
                {
                    string result = ProjectService.UpdateProject(project);
                    await CallbackAfterSubmit.InvokeAsync();
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        toast.sfSuccessToast.ShowAsync();

                        UpdateDialog();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        toast.sfErrorToast.ShowAsync();
                    }

                    UpdateDialog();

                }
                else
                {
                    string result = ProjectService.CreateProject(project);
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        toast.sfSuccessToast.ShowAsync();

                        UpdateDialog();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        toast.sfErrorToast.ShowAsync();
                    }

                    await CallbackAfterSubmit.InvokeAsync();

                }
            }
            else
            {
                toast.sfErrorToast.Title = editContext.GetValidationMessages().FirstOrDefault();
                toast.sfErrorToast.ShowAsync();

            }
        }


    }
}