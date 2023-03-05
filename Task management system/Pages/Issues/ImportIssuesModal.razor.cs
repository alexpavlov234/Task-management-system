using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json.Linq;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Kanban;
using Syncfusion.Blazor.Notifications;
using Syncfusion.ExcelExport;
using Syncfusion.XlsIO;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Projects;
using Task_management_system.Services;
using Task_management_system.Services.Common;
using static System.Net.Mime.MediaTypeNames;

namespace Task_management_system.Pages.Issues
{
    public partial class ImportIssuesModal
    {
        [Inject]
        private IKeyValueService keyValueService { get; set; }

        [Inject]
        private IIssueService IssueService { get; set; }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        private bool _isVisible = false;
        private List<Issue> issues = new List<Issue>();
        private ToastMsg toast = new ToastMsg();
        private IssueImportModal importIssueModal = new IssueImportModal();
        protected EditContext editContext;
        private Issue issue = new Issue();
        private string statusLineColor = "";
        private SfDropDownList<string, KeyValue> statusDropDownList = new SfDropDownList<string, KeyValue>();
        private SfDropDownList<string, KeyValue> priorityDropDownList = new SfDropDownList<string, KeyValue>();
        //public string? issueProjectName { get; set; }
        private List<KeyValue> statuses = new List<KeyValue>();
        private List<KeyValue> priorities = new List<KeyValue>();
        private SfGrid<Issue> sfGrid = new SfGrid<Issue>();
        private List<Project> projects { get; set; }
        private bool _isIssueNew = true;

        private Project project;
        private int _progressPercentage = 0;
        private int currentIssueIndex = 0;

        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }
        private ApplicationUser[] projectParticipants { get; set; }

        bool isLoggedUserAdmin = false;
        private bool _showOverlay = false;
        private ApplicationUser loggedUser { get; set; }
        public void OpenDialog(Project project)

        {
            this.issues.Clear();
            this.project = project;
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            loggedUser = UserService.GetLoggedUser();
            statuses = keyValueService.GetAllKeyValuesByKeyType("IssueStatus");
            priorities = keyValueService.GetAllKeyValuesByKeyType("IssuePriority");
            users = ProjectService.GetProjectById(this.issue.ProjectId).ProjectParticipants.ToList().Select(x => x.User).ToList();
            editContext = new EditContext(issue);
            _isVisible = true;
            StateHasChanged();
        }
        public async Task LoadQuestions(InputFileChangeEventArgs e)
        {

            var file = e.File;
            bool hasErrors = false;
            string message = "";
            if (file != null)
            {
                using (var stream = new MemoryStream())
                {
                    await file.OpenReadStream().CopyToAsync(stream);
                    stream.Position = 0;

                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;

                        // Open the Excel file
                        IWorkbook workbook = application.Workbooks.Open(stream);

                        // Get the first worksheet
                        IWorksheet worksheet = workbook.Worksheets[0];

                        // Get the data range
                        IRange dataRange = worksheet.UsedRange;

                        // Loop through the rows and map the data to Issue objects
                        for (int i = 2; i <= dataRange.Rows.Count()+1; i++)
                        {
                            string subject = dataRange[i, 1].Value.Trim();
                            string description = dataRange[i, 2].Value.Trim();
                            string assignedTo = dataRange[i, 3].Value.Trim();
                            string status = dataRange[i, 4].Value.Trim();
                            DateTime startTime = dataRange[i, 5].DateTime;
                            DateTime endTime = dataRange[i, 6].DateTime;
                            string priority = dataRange[i, 7].Value.Trim();

                           
                       
                            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(assignedTo) || string.IsNullOrEmpty(status) || string.IsNullOrEmpty(priority))
                            {
                                message = "Съществуват задачи/задача с празни данни! Невалидните данни са премахнати!";
                                hasErrors = true;
                                continue;
                            } else
                            {
                                bool isUserValid = false;
                                foreach (var participant in this.project.ProjectParticipants)
                                {
                                    if (participant.User.UserName == assignedTo)
                                    {
                                        isUserValid = true;
                                    }
                                }
                                if (!isUserValid)
                                {
                                    message = "Съществуват задачи/задача с невалидни потребители! Невалидните данни са премахнати!";
                                    hasErrors = true;
                                    continue;
                                }
                            }

                            Issue issue = new Issue
                            {
                                Subject = subject,
                                Description = description,
                                AssignedТo = new ApplicationUser { UserName = assignedTo },
                                Status = status,
                                StartTime = startTime,
                                EndTime = endTime,
                                Priority = priority
                            };

                            issue.AssignedТoId = (await UserService.GetApplicationUserByUsernameAsync(issue.AssignedТo.UserName)) != null ? (await UserService.GetApplicationUserByUsernameAsync(issue.AssignedТo.UserName)).Id : null;
                            issue.AssignedТo = await UserService.GetApplicationUserByIdAsync(issue.AssignedТoId);
                            issue.Assignee = UserService.GetLoggedUser();
                            issue.ProjectId = this.project.ProjectId;
                            issue.Location = "";
                            issue.RecurrenceException = "";
                            issue.RecurrenceRule = "";
                            issue.Subtasks = new List<Subtask>();
                            issues.Add(issue);
                        }

                        workbook.Close();
                    }
                }
            }

            if (hasErrors)
            {
                this.toast.sfErrorToast.Title = message;
                this.toast.sfErrorToast.ShowAsync();
            }
            this.sfGrid.Refresh();
            StateHasChanged();

        }

        private async Task UploadIssues()
        {


            if (issues.Any())
            {
                foreach (Issue issue in issues)
                {
                    IssueService.CreateIssue(issue);
                }
                this.toast.sfSuccessToast.Title = "Успешно импортиране!";
                this.toast.sfSuccessToast.ShowAsync();
                this.issues.Clear();
            }
            else
            {
                this.toast.sfErrorToast.Title = "Няма качени задачи!";
                this.toast.sfErrorToast.ShowAsync();

            }

            await CallbackAfterSubmit.InvokeAsync();
        }
        private void CloseDialog()
        {
            _isVisible = false;
            StateHasChanged();
        }

        private async Task DeleteIssue(Issue issue)
        {

            issues.Remove(issue);
            this.StateHasChanged();


        }

        private void UpdateData(Issue issue)
        {
            int index = issues.IndexOf(issue);
            issues[index] = issue;
            this.StateHasChanged();
        }
        private void EditIssue(Issue issue)
        {
            importIssueModal.OpenDialog(issue);
        }
    }
}

