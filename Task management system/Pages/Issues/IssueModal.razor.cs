using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages.Issues
{
    public partial class IssueModal
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

        private bool _isIssueNew = true;
        private bool _isVisible = false;
        private readonly DateTime MinDate = new DateTime(1900, 1, 1);
        bool isLoggedUserAdmin = false;

        private EditContext editContext;
        private Issue issue = new Issue();
        private string statusLineColor = "";
        private SfDropDownList<string, KeyValue> statusDropDownList = new SfDropDownList<string, KeyValue>();
        private SfDropDownList<string, KeyValue> priorityDropDownList = new SfDropDownList<string, KeyValue>();
        private List<KeyValue> statuses = new List<KeyValue>();
        private List<KeyValue> priorities = new List<KeyValue>();
        private SfGrid<Subtask> subtasksGrid = new SfGrid<Subtask>();
        private List<Project> projects { get; set; }
        private ToastMsg toast = new ToastMsg();
        private SubtaskModal subtaskModal = new SubtaskModal();
        private List<ApplicationUser> users { get; set; }
        private ApplicationUser[] projectParticipants { get; set; }
        private ApplicationUser loggedUser { get; set; }


        private void UpdateData()
        {
            if (isLoggedUserAdmin)
            {
                projects = ProjectService.GetAllProjects().OrderBy(x => x.ProjectName).ToList();
            }
            else
            {
                projects = ProjectService.GetAllProjects().Where(p => p.ProjectOwner.Id == loggedUser.Id
                                                                      || p.ProjectParticipants.Any(ap => ap.UserId == loggedUser.Id)).OrderBy(x => x.ProjectName).ToList();
            }
        }

        private async Task AddSubtask()
        {
            subtaskModal.OpenDialog(new Subtask { Status = "Нова", Location = "", RecurrenceException = "", RecurrenceRule = "", RecurrenceID = 0, Issue = issue, IssueId = issue.IssueId });
        }
        private async Task DeleteSubtask(Subtask subtask)
        {

            string result = IssueService.DeleteSubtask(subtask);
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
            if (issue != null)
            {
                _isIssueNew = IssueService.GetIssueById(issue.IssueId) == null;
                isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
                loggedUser = UserService.GetLoggedUser();
                UpdateData();
                statuses = new List<KeyValue>();
                priorities = KeyValueService.GetAllKeyValuesByKeyType("IssuePriority");
                this.issue = issue;
                if (_isIssueNew)
                {
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
                        this.issue.ProjectId = projects.First().ProjectId;
                        this.issue.StartTime = DateTime.Now;
                        this.issue.EndTime = DateTime.Now.AddMonths(1);
                    }
                }
                _ = GetStatus(this.issue.Status);

                users = ProjectService.GetProjectById(this.issue.ProjectId).ProjectParticipants.ToList().Select(x => x.User).ToList();
                editContext = new EditContext(issue);
                _isVisible = true;
                StateHasChanged();
            }
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

        private void OnValueSelectHandlerAssignedTo(ChangeEventArgs<string, KeyValue> args)
        {
            this.issue.AssignedТo = users.Find(x => x.Id == args.Value)!;
        }
        private void OnValueSelectHandlerStatus(ChangeEventArgs<string, KeyValue> args)
        {
            _ = GetStatus(args.Value);
        }
        private void CloseDialog()
        {
            _isVisible = false;

            this.CallbackAfterSubmit.InvokeAsync();
            StateHasChanged();
        }

        private async void SaveIssue()
        {

            
            if (editContext.Validate())

            {

                
                if (IssueService.GetIssueById(issue.IssueId) != null)
                {


                    //issue.Project = projects.Where(x => x.ProjectName == issueProjectName).First();
                    //issue.ProjectId = issue.Project.ProjectId;
                    var result = IssueService.UpdateIssue(issue);

                    await CallbackAfterSubmit.InvokeAsync();
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        _ = toast.sfSuccessToast.ShowAsync();
                        _isIssueNew = false;

                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        _ = toast.sfErrorToast.ShowAsync();
                    }

                    issue = IssueService.GetIssueById(issue.IssueId);
                    await subtasksGrid.Refresh();

                }
                else
                {


                    string result = IssueService.CreateIssue(issue);


                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        _ = toast.sfSuccessToast.ShowAsync();
                        _isIssueNew = false;

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


    }
}