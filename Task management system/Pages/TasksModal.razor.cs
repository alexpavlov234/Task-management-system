using KeyValue_management_system.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
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
        private List<KeyValue> statuses = new List<KeyValue>();
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
            statuses = new List<KeyValue>();
            this.issue = issue;


            if (issue.AssignedТo != null)
            {
                issueAssignedToUserName = issue.AssignedТo.UserName;
            }
            if (issue.Project != null)
            {
                this.issue.EndDate = this.issue.Project.EndDate;
                if (DateTime.Now > issue.EndDate)
                {
                    this.issue.StartDate = this.issue.Project.EndDate.AddMonths(-1);
                }
                else
                {
                    this.issue.StartDate = DateTime.Now;
                }
                issueProjectName = issue.Project.ProjectName;
            }
            else
            {
                issueProjectName = "";
            }
            GetStatus(this.issue.Status);
            this.projects = ProjectService.GetAllProjects();

            editContext = new EditContext(issue);
            IsVisible = true;
            StateHasChanged();
        }
        private void OnValueSelecthandler(SelectEventArgs<string> args)
        {
            this.issue.Project = this.projects.Where(x => x.ProjectName == issueProjectName).First();
            this.issue.EndDate = this.issue.Project.EndDate;
            if (DateTime.Now > issue.EndDate)
            {
                this.issue.StartDate = this.issue.Project.EndDate.AddMonths(-1);
            }
            else
            {
                this.issue.StartDate = DateTime.Now;
            }
        }
        private void CloseDialog()
        {
            IsVisible = false;
            StateHasChanged();
        }
        private void GetStatus(string status)
        {
            List<KeyValue> keyValues = keyValueService.GetAllKeyValuesByKeyType("IssueStatus");
            if (status == keyValues.Where(x => x.KeyValueIntCode == "New").First().Name)
            {
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "New" || x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "Closed").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "InExecution").First().Name)
            {
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "Closed").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "ForReview").First().Name)
            {
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "ForReview" || x.KeyValueIntCode == "Closed" || x.KeyValueIntCode == "ReturnedForCorrection").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "ReturnedForCorrection").First().Name)
            {
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "ReturnedForCorrection" || x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "ForReview").ToList());
            }
        }
        private async void SaveIssue()
        {


            if (editContext.Validate())

            {

                if (IssueService.GetTaskById(issue.IssueId) != null)
                {
                    issue.AssignedТo = await UserService.GetApplicationUserByUsernameAsync(issueAssignedToUserName);
                    
                    IssueService.UpdateTask(issue);

                    await CallbackAfterSubmit.InvokeAsync();
                    toast.sfSuccessToast.Title = "Успешно приложени промени!";
                    toast.sfSuccessToast.ShowAsync();

                }
                else
                {
                    issue.AssignedТo = await UserService.GetApplicationUserByUsernameAsync(issueAssignedToUserName);
                    //issue.Project = projects.Where(x => x.ProjectName == issueProjectName).First();
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