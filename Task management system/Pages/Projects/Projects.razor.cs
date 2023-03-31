using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages.Projects
{
    public partial class Projects
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
        private ApplicationUser loggedUser { get; set; }
        private List<KeyValue> projectTypes { get; set; }
        private List<Project> projects { get; set; }
        bool isLoggedUserAdmin = false;
        private ProjectModal projectModal = new();
        private ToastMsg toast = new();
        protected override async Task OnInitializedAsync()
        {
            projectTypes = KeyValueService.GetAllKeyValuesByKeyType("ProjectType");
            loggedUser = UserService.GetLoggedUser();
            isLoggedUserAdmin = UserService.IsLoggedUserAdmin();
            if (isLoggedUserAdmin)
            {
                projects = ProjectService.GetAllProjects();
            }
            else
            {
                projects = ProjectService.GetAllProjects().Where(p => p.ProjectOwner.Id == loggedUser.Id
                                                                      || p.ProjectParticipants.Any(ap => ap.UserId == loggedUser.Id)).ToList();
            }
        }
        private void AddNewProjectClickHandler()
        {
            projectModal.OpenDialog(new Project { StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), ProjectParticipants = null, ProjectOwner = null });
        }
        private void UpdateAfterProjectModalSubmitAsync()
        {
            projectTypes = KeyValueService.GetAllKeyValuesByKeyType("ProjectType");
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
            projectTypes = KeyValueService.GetAllKeyValuesByKeyType("ProjectType");
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
    }
}
