using System;
using Task_management_system.Areas.Identity;

public interface IUserService 
{
    ApplicationUser? GetApplicationUserByUsernameAsync(string Username);
    ApplicationUser? GetApplicationUserByIdAsync(string Username);
    void CreateApplicationUser(ApplicationUser applicationUser, string Password);
    List<ApplicationUser> GetAllUsers();
    void DeleteApplicationUser(ApplicationUser applicationUser);
    void UpdateApplicationUser(ApplicationUser applicationUser);
}
