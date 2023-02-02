using Task_management_system.Areas.Identity;
using Task_management_system.Models;

public interface IUserService
{
    Task<string> CreateApplicationUser(ApplicationUser applicationUser, String Password);
    Task DeleteApplicationUser(ApplicationUser applicationUser);
    List<ApplicationUser> GetAllUsers();
    Task<ApplicationUser> GetApplicationUserByIdAsync(string Id);
    Task<ApplicationUser> GetApplicationUserByUsernameAsync(string Username);
    Task UpdateApplicationUser(ApplicationUser applicationUser);
    Task<ApplicationUser> GetLoggedUser();

    Task<bool> IsLoggedUserAdmin();
    void Login(string username, string password, bool rememberMe);
    void Logout();
    Task AddRoleAsync(ApplicationUser user, string role);
    Task RemoveRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRoleAsync(ApplicationUser user);
    Task<ApplicationUser> ToExistingApplicationUser(InputModel inputModel);
    ApplicationUser ToApplicationUser(InputModel inputModel);
}
