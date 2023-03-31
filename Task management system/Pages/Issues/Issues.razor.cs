using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Kanban;
using Syncfusion.XlsIO;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages.Issues
{
    public partial class Issues
    {
        [Inject]
        private RoleManager<IdentityRole> RoleManager { get; set; }
        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; }
        [Inject]
        private SignInManager<ApplicationUser> SignInManager { get; set; }
        [Inject]
        private IKeyValueService KeyValueService { get; set; }
        [Inject]
        private IProjectService ProjectService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IIssueService IssueService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private List<Project> projects { get; set; }
        private List<Issue> issues { get; set; }
        private List<KeyValue> priorityTypes { get; set; }
        private SfKanban<Issue> sfKanban { get; set; }
        private IssueModal taskModal = new IssueModal();
        private ImportIssuesModal importIssuesModal = new ImportIssuesModal();
        private ToastMsg toast = new ToastMsg();
        private readonly string[] tabsClasses = new string[3];
        public int TabId { get; set; }
        private bool isLoggedUserAdmin = false;
        private ApplicationUser loggedUser { get; set; }
        public bool isLoadingData { get; set; } = false;
        private readonly List<Syncfusion.Blazor.Kanban.ColumnModel> columnData = new List<Syncfusion.Blazor.Kanban.ColumnModel>() {
        new Syncfusion.Blazor.Kanban.ColumnModel(){ HeaderText= "Нова", KeyField= new List<string>() { "Нова" } },
        new Syncfusion.Blazor.Kanban.ColumnModel(){ HeaderText= "В изпълнение", KeyField= new List<string>() { "В изпълнение" } },
        new Syncfusion.Blazor.Kanban.ColumnModel(){ HeaderText= "За преглед", KeyField= new List<string>() { "За преглед" } },
        new Syncfusion.Blazor.Kanban.ColumnModel(){ HeaderText= "Върната за корекция", KeyField=new List<string>() { "Върната за корекция" } },
        new Syncfusion.Blazor.Kanban.ColumnModel(){ HeaderText= "Затворена", KeyField=new List<string>() { "Затворена" } }
    };
        private void UpdateData()
        {
            if (isLoggedUserAdmin)
            {
                projects = ProjectService.GetAllProjects();
                issues = IssueService.GetAllIssues();
            }
            else
            {
                projects = ProjectService.GetAllProjects().Where(p => p.ProjectOwner.Id == loggedUser.Id
                                                                      || p.ProjectParticipants.Any(ap => ap.UserId == loggedUser.Id)).ToList();
                issues = IssueService.GetAllIssues()
          .Where(i => projects.Any(p => p.ProjectId == i.ProjectId) && (i.AssignedТo.Id == loggedUser.Id || i.Assignee.Id == loggedUser.Id))
          .ToList();
            }
        }
        private void ChangeTab(int index)
        {
            switch (index)
            {
                case 1:
                    tabsClasses[0] = "nav-link active";
                    tabsClasses[1] = "nav-link";
                    tabsClasses[2] = "nav-link";
                    TabId = 1; break;
                case 2:
                    tabsClasses[1] = "nav-link active";
                    tabsClasses[0] = "nav-link";
                    tabsClasses[2] = "nav-link";
                    TabId = 2; break;
                case 3:
                    tabsClasses[2] = "nav-link active";
                    tabsClasses[0] = "nav-link";
                    tabsClasses[1] = "nav-link";
                    TabId = 3; break;
            }
        }
        protected override Task OnInitializedAsync()
        {
            tabsClasses[0] = "nav-link active";
            tabsClasses[1] = "nav-link";
            tabsClasses[2] = "nav-link";
            TabId = 1;
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            loggedUser = UserService.GetLoggedUser();
            UpdateData();
            priorityTypes = KeyValueService.GetAllKeyValuesByKeyType("IssuePriority");
            return Task.CompletedTask;
        }
        public async void ActionCompleteHandler(Syncfusion.Blazor.Kanban.ActionEventArgs<Issue> args)
        {
            string result = IssueService.UpdateIssue(args.ChangedRecords.FirstOrDefault());
            if (result.StartsWith("Успешно"))
            {
                toast.sfSuccessToast.Title = result;
                _ = toast.sfSuccessToast.ShowAsync();
                UpdateData();
                await sfKanban.RefreshAsync();
                StateHasChanged();
            }
            else
            {
                toast.sfErrorToast.Title = result;
                _ = toast.sfErrorToast.ShowAsync();
            }
        }
        private async Task AddNewTaskClickHandler()
        {
            if (projects.Count() == 0)
            {
                toast.sfErrorToast.Title = "Моля създайте първо проект!";
                _ = toast.sfErrorToast.ShowAsync();
            }
            else
            {
                taskModal.OpenDialog(new Issue { Status = "Нова", Assignee = UserService.GetLoggedUser(), Subtasks = new List<Subtask>(), Location = "", RecurrenceException = "", RecurrenceRule = "", RecurrenceID = 0, Priority = "Нормален" });
            }
        }
        private async Task AddIssueToProject(Project project)
        {
            taskModal.OpenDialog(new Issue { Status = "Нова", Project = project, ProjectId = project.ProjectId, Assignee = UserService.GetLoggedUser(), Subtasks = new List<Subtask>(), Location = "", RecurrenceException = "", RecurrenceRule = "", RecurrenceID = 0, Priority = "Нормален" });
        }
        private async Task ImportIssues(Project project)
        {
            importIssuesModal.OpenDialog(project);
        }
        public async Task ExportIssues(Project project)
        {
            using ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;
            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet worksheet = workbook.Worksheets[0];
            worksheet.Range["A1"].Text = "Име";
            worksheet.Range["B1"].Text = "Описание";
            worksheet.Range["C1"].Text = "Възложена на";
            worksheet.Range["D1"].Text = "Статус";
            worksheet.Range["E1"].Text = "Начална дата";
            worksheet.Range["F1"].Text = "Крайна дата";
            worksheet.Range["G1"].Text = "Приоритет";
            for (int i = 0; i < project.Issues.Count(); i++)
            {
                Issue issue = project.Issues.ElementAt(i);
                worksheet.Range["A" + (i + 2)].Text = issue.Subject;
                worksheet.Range["B" + (i + 2)].Text = issue.Description;
                worksheet.Range["C" + (i + 2)].Text = issue.AssignedТo.UserName;
                worksheet.Range["D" + (i + 2)].Text = issue.Status;
                worksheet.Range["E" + (i + 2)].DateTime = issue.StartTime;
                worksheet.Range["F" + (i + 2)].DateTime = issue.EndTime;
                worksheet.Range["G" + (i + 2)].Text = issue.Priority;
            }
            worksheet.UsedRange.AutofitColumns();
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            _ = await JSRuntime.InvokeAsync<object>("saveAsFile", project.ProjectName.Replace(" ", "_").Replace("-", "_") + "_Задачи.xlsx", Convert.ToBase64String(stream.ToArray()));
            workbook.Close();
        }
        private void OnDialogOpen(DialogOpenEventArgs<Issue> args)
        {
            args.Cancel = true;
        }
        private async Task UpdateAfterIssueModalSubmitAsync()
        {
            UpdateData();
            if (TabId == 2)
            {
                await sfKanban.RefreshAsync();
            }
            StateHasChanged();
        }
        private async Task UpdateAfterImportIssuesModalSubmitAsync()
        {
            UpdateData();
            if (TabId == 2)
            {
                await sfKanban.RefreshAsync();
            }
            StateHasChanged();
        }
        private async Task DeleteIssue(Issue issue)
        {
            string result = IssueService.DeleteIssue(issue);
            if (result.StartsWith("Успешно"))
            {
                toast.sfSuccessToast.Title = result;
                _ = toast.sfSuccessToast.ShowAsync();
                UpdateData();
                if (TabId == 2)
                {
                    await sfKanban.RefreshAsync();
                }
                StateHasChanged();
            }
            else
            {
                toast.sfErrorToast.Title = result;
                _ = toast.sfErrorToast.ShowAsync();
            }
            StateHasChanged();
        }
        private void EditIssue(Issue issue)
        {
            taskModal.OpenDialog(issue);
        }
    }
}
