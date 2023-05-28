using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.XlsIO;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Shared;
namespace Task_management_system.Pages.Issues
{
    public partial class ImportIssuesModal
    {
        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }
        [Inject]
        private IProjectService ProjectService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IKeyValueService KeyValueService { get; set; }
        [Inject]
        private IIssueService IssueService { get; set; }
        private bool _isVisible = false;
        private readonly List<Issue> issues = new List<Issue>();
        private ToastMsg toast = new ToastMsg();
        private IssueImportModal importIssueModal = new IssueImportModal();
        protected EditContext editContext;
        private readonly Issue issue = new Issue();
        private readonly string statusLineColor = "";
        private readonly SfDropDownList<string, KeyValue> statusDropDownList = new SfDropDownList<string, KeyValue>();
        private readonly SfDropDownList<string, KeyValue> priorityDropDownList = new SfDropDownList<string, KeyValue>();
        List<KeyType> keyTypes;
        KeyValue[] keyValues;
        private List<KeyValue> statuses = new List<KeyValue>();
        private List<KeyValue> priorities = new List<KeyValue>();
        private readonly SfGrid<Issue> sfGrid = new SfGrid<Issue>();
        private List<Project> projects { get; set; }
        private readonly bool _isIssueNew = true;
        private Project project;
        private readonly int _progressPercentage = 0;
        private readonly int currentIssueIndex = 0;
        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }
        private ApplicationUser[] projectParticipants { get; set; }
        bool isLoggedUserAdmin = false;
        private readonly bool _showOverlay = false;
        private ApplicationUser loggedUser { get; set; }
        public void OpenDialog(Project project)
        {
            issues.Clear();
            this.project = project;
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            loggedUser = UserService.GetLoggedUser();
            statuses = KeyValueService.GetAllKeyValuesByKeyType("IssueStatus");
            priorities = KeyValueService.GetAllKeyValuesByKeyType("IssuePriority");
            users = ProjectService.GetProjectById(this.project.ProjectId).ProjectParticipants.ToList().Select(x => x.User).ToList();
            editContext = new EditContext(issue);
            keyTypes = new List<KeyType>()
            {
                new KeyType { KeyTypeName = "Тип на проект", KeyTypeIntCode = "ProjectType", Description = "Тип на проекта", IsSystem = true },
                new KeyType { KeyTypeName = "Статус на задача", KeyTypeIntCode = "IssueStatus", Description = "Статус на задача", IsSystem = true },
                new KeyType { KeyTypeName = "Приоритет на задача", KeyTypeIntCode = "IssuePriority", Description = "Приоритет на задача", IsSystem = true }
            };
            keyValues = new KeyValue[]
            {
                new KeyValue { KeyType = keyTypes[1], Name = "Нова", NameEN = "New", IsActive = true, KeyValueIntCode = "New", Description = "Нова" },
                new KeyValue { KeyType = keyTypes[1], Name = "В изпълнение", NameEN = "In execution", IsActive = true, KeyValueIntCode = "InExecution", Description = "В изпълнение" },
                new KeyValue { KeyType = keyTypes[1], Name = "За преглед", NameEN = "For review", IsActive = true, KeyValueIntCode = "ForReview", Description = "За преглед" },
                new KeyValue { KeyType = keyTypes[1], Name = "Затворена", NameEN = "Closed", IsActive = true, KeyValueIntCode = "Closed", Description = "Затворена" },
                new KeyValue { KeyType = keyTypes[1], Name = "Върната за корекция", NameEN = "Returned for correction", IsActive = true, KeyValueIntCode = "ReturnedForCorrection", Description = "Върната за корекция" },
                new KeyValue { KeyType = keyTypes[0], Name = "Работен", NameEN = "Working", IsActive = true, KeyValueIntCode = "Working", Description = "Проект свързан с работа" },
                new KeyValue { KeyType = keyTypes[0], Name = "Учебен", NameEN = "Educational", IsActive = true, KeyValueIntCode = "Educational", Description = "Проект свързан с учебен процес/училище" },
                new KeyValue { KeyType = keyTypes[0], Name = "Личен", NameEN = "Personal", IsActive = true, KeyValueIntCode = "Personal", Description = "Проект свързан с личния живот" },
                new KeyValue { KeyType = keyTypes[2], Name = "Висок", NameEN = "High", IsActive = true, KeyValueIntCode = "High", Description = "Висок приоритет" },
                new KeyValue { KeyType = keyTypes[2], Name = "Нисък", NameEN = "Low", IsActive = true, KeyValueIntCode = "Low", Description = "Нисък приоритет" },
                new KeyValue { KeyType = keyTypes[2], Name = "Нормален", NameEN = "Normal", IsActive = true, KeyValueIntCode = "Normal", Description = "Нормален приоритет" }
            };
            _isVisible = true;
            StateHasChanged();
        }
        public async Task LoadQuestions(InputFileChangeEventArgs e)
        {
            IBrowserFile file = e.File;
            bool hasErrors = false;
            string message = "";
            if (file != null)
            {
                using MemoryStream stream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(stream);
                stream.Position = 0;
                using ExcelEngine excelEngine = new ExcelEngine();
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
                    try
                    {
                        string subject = dataRange[i, 1].Value.Trim();
                        string description = dataRange[i, 2].Value.Trim();
                        string assignedTo = dataRange[i, 3].Value.Trim();
                        string status = dataRange[i, 4].Value.Trim();
                        DateTime startTime = dataRange[i, 5].DateTime;
                        DateTime endTime = dataRange[i, 6].DateTime;
                        string priority = dataRange[i, 7].Value.Trim();
                        if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(assignedTo) || string.IsNullOrEmpty(status) || string.IsNullOrEmpty(priority) || dataRange[i, 5].DateTime == DateTime.MinValue || dataRange[i, 6].DateTime == DateTime.MinValue)
                        {
                            message = "Съществуват задачи/задача с празни данни! Невалидните данни са премахнати!";
                            hasErrors = true;
                            continue;
                        }
                        else
                        {
                            bool isUserValid = false;
                            foreach (ApplicationUserProject participant in project.ProjectParticipants)
                            {
                                if (participant.User.UserName == assignedTo)
                                {
                                    isUserValid = true;
                                }
                            }
                            bool isStatusValid = false;
                            bool isPriorityValid = false;
                            for (int j = 0; j < keyValues.Length; j++)
                            {
                                if (keyValues[j].Name == status)
                                {
                                    isStatusValid = true;
                                }
                                if (keyValues[j].Name == priority)
                                {
                                    isPriorityValid = true;
                                }
                            }
                            if (!isUserValid)
                            {
                                message = "Съществуват задачи/задача с невалидни потребители! Невалидните данни са премахнати!";
                                hasErrors = true;
                                continue;
                            }
                            else if (!isStatusValid || !isPriorityValid)
                            {
                                message = "Невалидните данни са премахнати!";
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
                        issue.ProjectId = project.ProjectId;
                        issue.Location = "";
                        issue.RecurrenceException = "";
                        issue.RecurrenceRule = "";
                        issue.Subtasks = new List<Subtask>();
                        issues.Add(issue);
                    }
                    catch
                    {
                        message = "Невалидните данни са премахнати!";
                        hasErrors = true;
                        continue;
                    }
                }
                workbook.Close();
            }
            if (hasErrors)
            {
                toast.sfErrorToast.Title = message;
                _ = toast.sfErrorToast.ShowAsync();
            }
            _ = sfGrid.Refresh();
            StateHasChanged();
        }
        private async Task UploadIssues()
        {
            if (issues.Any())
            {
                foreach (Issue issue in issues)
                {
                    _ = IssueService.CreateIssue(issue);
                }
                toast.sfSuccessToast.Title = "Успешно импортиране!";
                _ = toast.sfSuccessToast.ShowAsync();
                issues.Clear();
            }
            else
            {
                toast.sfErrorToast.Title = "Няма качени задачи!";
                _ = toast.sfErrorToast.ShowAsync();
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
            _ = issues.Remove(issue);
            StateHasChanged();
        }
        private void UpdateData(Issue issue)
        {
            int index = issues.IndexOf(issue);
            issues[index] = issue;
            StateHasChanged();
        }
        private void EditIssue(Issue issue)
        {
            importIssueModal.OpenDialog(issue);
        }
    }
}
