using KeyValue_management_system.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Common;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages
{
    public partial class TasksModal
    {
        protected EditContext editContext;
        private Issue issue = new Issue();
        public string? issueAssignedToUserName { get; set; }
        public string? issueProjectName { get; set; }
        private List<Project> projects { get; set; }
        private bool IsUserNew = false;
        private bool IsVisible = false;
        private DateTime MinDate = new DateTime(1900, 1, 1);
        private ToastMsg toast = new ToastMsg();
        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }
        private ApplicationUser[] projectParticipants { get; set; }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        private BaseHelper BaseHelper { get; set; }

        [Inject]
        private IKeyValueService keyValueService { get; set; }

        [Inject]
        private IUserService UserService { get; set; }

        [Inject]
        private IIssueService IssueService { get; set; }


        protected async override Task OnInitializedAsync()
        {
            projectTypes = keyValueService.GetAllKeyValuesByKeyType("IssueType");
            users = UserService.GetAllUsers();
        }
        public async void OpenDialog(Issue issue)

        {
            this.issue = issue;
            if (issue.AssignedТo != null)
            {
                issueAssignedToUserName = issue.AssignedТo.UserName;
            }
            if(issue.Project != null)
            {
                issueProjectName = issue.Project.ProjectName;
            }
            this.projects = ProjectService.GetAllProjects();
            
            editContext = new EditContext(issue);
            IsVisible = true;
            StateHasChanged();
        }

        private void CloseDialog()
        {
            IsVisible = false;
            StateHasChanged();
        }

        private async void SaveIssue()
        {
          
             
            if (editContext.Validate())

            {

                if (IssueService.GetTaskById(issue.IssueId) != null)
                {
                    issue.AssignedТo = await UserService.GetApplicationUserByUsernameAsync(issueAssignedToUserName);
                    issue.Project = projects.Where(x => x.ProjectName == issueProjectName).First();
                    
                    IssueService.UpdateTask(issue);
                  
                    await CallbackAfterSubmit.InvokeAsync();
                    toast.sfSuccessToast.Title = "Успешно приложени промени!";
                    toast.sfSuccessToast.ShowAsync();
                    
                }
                else
                {
                    issue.AssignedТo = await UserService.GetApplicationUserByUsernameAsync(issueAssignedToUserName);
                    issue.Project = projects.Where(x => x.ProjectName == issueProjectName).First();
                    string result = IssueService.CreateTask(issue);
        
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        toast.sfSuccessToast.ShowAsync();
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