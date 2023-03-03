using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
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
                        for (int i = 2; i <= dataRange.Rows.Count(); i++)
                        {
                            Issue issue = new Issue
                            {
                                Subject = dataRange[i, 1].Value,
                                Description = dataRange[i, 2].Value,
                                AssignedТo = new ApplicationUser {UserName = dataRange[i, 3].Value},
                                Status = dataRange[i, 4].Value,
                                StartTime = dataRange[i, 5].DateTime,
                                EndTime = dataRange[i, 6].DateTime,
                                Priority = dataRange[i, 7].Value
                                
                                
                            };
                            issue.AssignedТoId = (await UserService.GetApplicationUserByUsernameAsync(issue.AssignedТo.UserName)) != null ? (await UserService.GetApplicationUserByUsernameAsync(issue.AssignedТo.UserName)).Id : null;
                            issue.AssignedТo = null;
                            issue.Assignee = UserService.GetLoggedUser();
                            issue.ProjectId = this.project.ProjectId;
                            issues.Add(issue);
                        }


                        workbook.Close();
                    }
                }
            }
            else
            {


            }

            this.sfGrid.Refresh();
            StateHasChanged();

        }

        private async Task UploadIssues()
        {
            _showOverlay = true;
            int currentQuestion;
            foreach (Issue issue in issues)
            {
                currentQuestion = issues.IndexOf(issue) + 1;
                currentIssueIndex = currentQuestion;
               _progressPercentage = (int)((float)currentQuestion / issues.Count() * 100); 
                IssueService.CreateIssue(issue);
            }
            _showOverlay = false;
            await CallbackAfterSubmit.InvokeAsync();
        }
        private void CloseDialog()
        {
            _isVisible = false;
            StateHasChanged();
        }
    }
}

