using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Common;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages
{
    public partial class IssueModal
    {
        protected EditContext editContext;
        private Issue issue = new Issue();
        private string statusLineColor = "";
        private SfDropDownList<string, KeyValue> statusDropDownList = new SfDropDownList<string, KeyValue>();
        private SfDropDownList<string, KeyValue> priorityDropDownList = new SfDropDownList<string, KeyValue>();
        public string? issueAssignedToUserName { get; set; }
        //public string? issueProjectName { get; set; }
        private List<KeyValue> statuses = new List<KeyValue>();
        private List<KeyValue> priorities = new List<KeyValue>();
        private SfGrid<Subtask> subtasksGrid = new SfGrid<Subtask>();
        private List<Project> projects { get; set; }
        private bool IsIssueNew = true;
        private bool IsVisible = false;
        private DateTime MinDate = new DateTime(1900, 1, 1);
        private ToastMsg toast = new ToastMsg();
        private SubtaskModal subtaskModal = new SubtaskModal();
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
        private async Task AddSubtask()
        {
            subtaskModal.OpenDialog(new Subtask { Status = "Нова", Location = "", RecurrenceException = "", RecurrenceRule = "", RecurrenceID = 0, Issue = issue, IssueId = issue.IssueId });

            //managementModal.OpenDialog(new ApplicationUser());
        }
        private async Task DeleteSubtask(Subtask subtask)
        {

            string result = IssueService.DeleteSubtask(subtask);
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
            issue = IssueService.GetIssueById(issue.IssueId);
            await CallbackAfterSubmit.InvokeAsync();
            await subtasksGrid.Refresh();
            StateHasChanged();


        }

        private async Task EditSubtask(Subtask subtask)
        {

            subtaskModal.OpenDialog(subtask);
        }
        private async Task UpdateAfterSubtaskModalSubmitAsync()
        {

            issue = IssueService.GetIssueById(issue.IssueId);
            await subtasksGrid.Refresh();
            StateHasChanged();
        }
        public async void OpenDialog(Issue issue)

        {
            IsIssueNew = IssueService.GetIssueById(issue.IssueId) == null;
            statuses = new List<KeyValue>();
            priorities = keyValueService.GetAllKeyValuesByKeyType("IssuePriority");
            this.issue = issue;
            if (issue.AssignedТo != null)
            {
                issueAssignedToUserName = issue.AssignedТo.UserName;
            }
            if (issue.Project != null)
            {
                //issueProjectName = issue.Project.ProjectName;
                this.issue.EndTime = this.issue.Project.EndDate;

                if (DateTime.Now < issue.EndTime)
                {
                    this.issue.StartTime = this.issue.Project.EndDate.AddMonths(-1);
                }
                else
                {

                    this.issue.StartTime = DateTime.Now;
                    this.issue.EndTime = DateTime.Now.AddMonths(1);
                }
            }
            else
            {

                //issueProjectName = "";
                this.issue.StartTime = DateTime.Now;
                this.issue.EndTime = DateTime.Now.AddMonths(1);
            }
            GetStatus(this.issue.Status);
            projects = ProjectService.GetAllProjects();

            editContext = new EditContext(issue);
            IsVisible = true;
            StateHasChanged();
        }
        private async Task GetStatus(string status)
        {
            statuses.Clear();

            List<KeyValue> keyValues = keyValueService.GetAllKeyValuesByKeyType("IssueStatus");
            if (status == keyValues.Where(x => x.KeyValueIntCode == "New").First().Name)
            {
                statusLineColor = "task-line-blue";
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "New" || x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "Closed").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "InExecution").First().Name)
            {
                statusLineColor = "task-line-green";
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "Closed" || x.KeyValueIntCode == "ForReview").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "ForReview").First().Name)
            {
                statusLineColor = "task-line-yellow";
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "ForReview" || x.KeyValueIntCode == "Closed" || x.KeyValueIntCode == "ReturnedForCorrection").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "ReturnedForCorrection").First().Name)
            {
                statusLineColor = "task-line-red";
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "ReturnedForCorrection" || x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "ForReview").ToList());
            }
            else if (status == keyValues.Where(x => x.KeyValueIntCode == "Closed").First().Name)
            {
                statuses.AddRange(keyValues.Where(x => x.KeyValueIntCode == "New" || x.KeyValueIntCode == "InExecution" || x.KeyValueIntCode == "Closed").ToList());
                statusLineColor = "task-line-gray";
            }

            await statusDropDownList.RefreshDataAsync();

            StateHasChanged();
        }

        private void OnValueSelectHandlerProject(ChangeEventArgs<int, Project> args)
        {
            issue.Project = projects.Where(x => x.ProjectId == args.Value).First();
            issue.EndTime = issue.Project.EndDate;
            if (DateTime.Now < issue.EndTime)
            {
                issue.StartTime = issue.Project.EndDate.AddMonths(-1);
            }
            else
            {
                issue.StartTime = DateTime.Now;
                issue.EndTime = DateTime.Now.AddMonths(1);
            }
        }
        private void OnValueSelectHandlerStatus(ChangeEventArgs<string, KeyValue> args)
        {
            GetStatus(args.Value);
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


                if (IssueService.GetIssueById(issue.IssueId) != null)
                {
                    if (issueAssignedToUserName != null)
                    {
                        issue.AssignedТo = await UserService.GetApplicationUserByUsernameAsync(issueAssignedToUserName);
                    }

                    //issue.Project = projects.Where(x => x.ProjectName == issueProjectName).First();
                    //issue.ProjectId = issue.Project.ProjectId;
                    IssueService.UpdateIssue(issue);

                    await CallbackAfterSubmit.InvokeAsync();
                    toast.sfSuccessToast.Title = "Успешно приложени промени!";
                    toast.sfSuccessToast.ShowAsync();
                    issue = IssueService.GetIssueById(issue.IssueId);
                    await subtasksGrid.Refresh();

                }
                else
                {
                    if (issueAssignedToUserName != null)
                    {
                        issue.AssignedТo = await UserService.GetApplicationUserByUsernameAsync(issueAssignedToUserName);
                    }


                    string result = IssueService.CreateIssue(issue);


                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        toast.sfSuccessToast.ShowAsync();
                        IsIssueNew = false;

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