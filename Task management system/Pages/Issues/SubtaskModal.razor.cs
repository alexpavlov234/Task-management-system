using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages.Issues
{
    public partial class SubtaskModal
    {
        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        private IProjectService ProjectService { get; set; }
        [Inject]
        private IKeyValueService KeyValueService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IIssueService IssueService { get; set; }
        public string? issueAssignedToUserName { get; set; }
        protected EditContext editContext;
        private Subtask subtask = new Subtask();
        private readonly bool IsUserNew = false;
        private bool IsVisible = false;
        private readonly DateTime MinDate = new DateTime(1900, 1, 1);
        private ToastMsg toast = new ToastMsg();
        private string statusLineColor = "";
        private SfDropDownList<string, KeyValue> statusDropDownList = new SfDropDownList<string, KeyValue>();
        private List<KeyValue> statuses = new List<KeyValue>();
        private List<Project> projects { get; set; }
        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }
        private ApplicationUser[] projectParticipants { get; set; }

        protected async override Task OnInitializedAsync()
        {
            projectTypes = KeyValueService.GetAllKeyValuesByKeyType("IssueType");
            users = UserService.GetAllUsers();
        }

        public async void OpenDialog(Subtask subtask)

        {
            statuses = new List<KeyValue>();
            this.subtask = subtask;

            if (subtask.Issue != null)
            {
                //issueProjectName = subtask.Project.ProjectName;
                this.subtask.EndTime = this.subtask.Issue.EndTime;
                if (DateTime.Now < subtask.EndTime)
                {
                    this.subtask.StartTime = this.subtask.Issue.EndTime.AddMonths(-1);
                }
                else
                {
                    this.subtask.StartTime = DateTime.Now;
                    this.subtask.EndTime = DateTime.Now.AddMonths(1);
                }
            }
            else
            {
                //issueProjectName = "";
                this.subtask.StartTime = DateTime.Now;
                this.subtask.EndTime = DateTime.Now.AddMonths(1);
            }
            _ = GetStatus(this.subtask.Status);
            projects = ProjectService.GetAllProjects();

            editContext = new EditContext(subtask);
            IsVisible = true;
            StateHasChanged();
        }
        private async Task GetStatus(string status)
        {
            statuses.Clear();

            List<KeyValue> keyValues = KeyValueService.GetAllKeyValuesByKeyType("IssueStatus");
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

        private void OnValueSelectHandlerStatus(ChangeEventArgs<string, KeyValue> args)
        {
            _ = GetStatus(args.Value);
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
                if (IssueService.GetSubtaskById(subtask.SubtaskId) != null)
                {
                    string result = IssueService.UpdateSubtask(subtask);
                    await CallbackAfterSubmit.InvokeAsync();
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        _ = toast.sfSuccessToast.ShowAsync();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        _ = toast.sfErrorToast.ShowAsync();
                    }
                }
                else
                {
                    string result = IssueService.CreateSubtask(subtask);
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        _ = toast.sfSuccessToast.ShowAsync();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        _ = toast.sfErrorToast.ShowAsync();
                    }
                    await CallbackAfterSubmit.InvokeAsync();
                }
            }
        }
    }
}