using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Issues;
using Task_management_system.Pages.Projects;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages
{
    public partial class Search
    {
        [Parameter]
        public bool ShowHeader { get; set; } = true;
        [Inject]
        private RoleManager<IdentityRole> RoleManager { get; set; }
        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; }
        [Inject]
        private SignInManager<ApplicationUser> SignInManager { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IKeyValueService KeyValueService { get; set; }
        [Inject]
        private IProjectService ProjectService { get; set; }
        [Inject]
        private IIssueService IssueService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private List<Project> projects { get; set; }
        private List<Issue> issues { get; set; }
        private List<object> itemsResult { get; set; }
        private ApplicationUser loggedUser { get; set; }
        bool isLoggedUserAdmin = false;
        readonly bool editProject = false;
        private string query = "";
        private ProjectModal projectModal = new();
        private ToastMsg toast = new();
        private IssueModal taskModal = new IssueModal();
        protected override async Task OnInitializedAsync()
        {
            loggedUser = UserService.GetLoggedUser();
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            UpdateData();
            string queryString = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
            System.Collections.Specialized.NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(queryString);
            string? query = queryParams["q"];
            if (!string.IsNullOrEmpty(query))
            {
                // Remove the leading '?' from the query string
                string searchQuery = query;
                // Set the search field value
                this.query = searchQuery;
                StateHasChanged();
                // Perform the search
                SearchInput();
            }
        }
        private bool IsProjectEditable(Project project)
        {
            return isLoggedUserAdmin || project.ProjectOwner.Id == loggedUser.Id;
        }
        private void UpdateData()
        {
            itemsResult = new List<object>();
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
        private async Task UpdateAfterIssueModalSubmitAsync()
        {
            UpdateData();
            StateHasChanged();
        }
        private void UpdateAfterManagementModalSubmitAsync()
        {
            UpdateData();
            StateHasChanged();
        }
        private async Task DeleteIssue(Issue issue)
        {
            string result = IssueService.DeleteIssue(issue);
            if (result.StartsWith("Успешно"))
            {
                toast.sfSuccessToast.Title = result;
                _ = toast.sfSuccessToast.ShowAsync();
                StateHasChanged();
            }
            else
            {
                toast.sfErrorToast.Title = result;
                _ = toast.sfErrorToast.ShowAsync();
            }
            projects = ProjectService.GetAllProjects();
            StateHasChanged();
        }
        private void EditIssue(Issue issue)
        {
            taskModal.OpenDialog(issue);
        }
        private void DeleteProject(Project project)
        {
            string result = ProjectService.DeleteProject(project.ProjectId);
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
            if (isLoggedUserAdmin)
            {
                projects = ProjectService.GetAllProjects();
            }
            else
            {
                projects = ProjectService.GetAllProjects().Where(p => p.ProjectOwner.Id == loggedUser.Id
                                                                      || p.ProjectParticipants.Any(ap => ap.UserId == loggedUser.Id)).ToList();
            }
            StateHasChanged();
        }
        private void EditProject(Project project)
        {
            projectModal.OpenDialog(project);
        }
        public void SearchInput()
        {
            if (query != "")
            {
                if (ShowHeader)
                {
                    List<object> allItems = new List<object>();
                    allItems.AddRange(projects.Cast<object>());
                    allItems.AddRange(issues.Cast<object>());
                    itemsResult = allItems.Where(item =>
                    {
                        if (item is Project project)
                        {
                            return project.ProjectName.ToLower().Contains(query.ToLower()) || project.ProjectDescription.ToLower().Contains(query.ToLower());
                        }
                        else if (item is Issue issue)
                        {
                            return issue.Subject.ToLower().Contains(query.ToLower()) || issue.Description.ToLower().Contains(query.ToLower());
                        }
                        else
                        {
                            return false;
                        }
                    }).ToList();
                    StateHasChanged();
                }
                else
                {
                    NavigationManager.NavigateTo($"/search?q={query}");
                }
            }
        }
    }
}
