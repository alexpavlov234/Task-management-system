using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;

namespace Task_management_system.Services;

public class UserService : Controller, IUserService
{

    private readonly Context _context;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(Context context, UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _httpContextAccessor = httpContextAccessor;
    }

    // Създаване на нов потребител
    public async Task<string> CreateApplicationUser(ApplicationUser applicationUser, String Password)
    {
        _context.DetachAllEntities();
        applicationUser.FirstName.Trim();
        applicationUser.LastName.Trim();
        // Използване на метода CreateAsync от UserManager за създаване на нов потребител
        IdentityResult result = await _userManager.CreateAsync(applicationUser, Password);
        // Отделяне на потребителя от локалния контекст

        if (!result.Succeeded)
        {
            // Връщане на първата грешка от резултата
            return result.Errors.ToList().First().Description;
        }
        else
        {
            _ = await _userManager.AddToRoleAsync(applicationUser, "User");
            // Успешно създаден потребител
            return "Успешно създаден потребител!";
        }
    }

    // Изтриване на потребител
    public async Task<string> DeleteApplicationUser(ApplicationUser applicationUser)
    {
        try
        {
            _context.DetachAllEntities();
            // Търсене на потребителя в локалния контекст
            ApplicationUser? local = _context.Set<ApplicationUser>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(applicationUser.Id));

            // Проверка дали local не е null
            if (local != null)
            {
                // Отделяне
                _context.Entry(local).State = EntityState.Detached;
            }

            // Задаване на флаг за изтриване
            _context.Entry(applicationUser).State = EntityState.Deleted;

            // Запазване на промените
            _ = _context.SaveChanges();
            return "Успешно изтриване!";
        }
        catch (DbUpdateException)
        {
            return "Неуспешно изтриване, понеже потребителят е свързан с проект/задача!";
        }
        catch
        {
            return "Неуспешно изтриване!";
        }
    }

    // Връща списък с всички потребители
    public List<ApplicationUser> GetAllUsers()
    {
        List<ApplicationUser> users = _context.Users.AsNoTracking().ToList();
        return users;
    }

    // Връща един потребител по зададен Id
    public async Task<ApplicationUser> GetApplicationUserByIdAsync(string Id)
    {
        _context.DetachAllEntities();
        // Търси в локалната база данни за потребител с зададеното Id
        ApplicationUser? local = _context.Users.AsNoTracking().FirstOrDefault(entry => entry.Id.Equals(Id));

        return local;
    }

    public async Task<ApplicationUser> GetApplicationUserByUsernameAsync(string Username)
    {
        _context.DetachAllEntities();
        ApplicationUser? local = _context.Set<ApplicationUser>().Local.FirstOrDefault(entry => entry.UserName.Equals(Username));
        // check if local is not null
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        ApplicationUser user = _context.Users.Where(x => x.UserName == Username).AsNoTracking().FirstOrDefault();// await _userManager.FindByNameAsync(Username);
        return user;
    }

    public async Task UpdateApplicationUser(ApplicationUser updatedUser)
    {
        // Retrieve the user from the database using its key value
        var user = await _userManager.FindByIdAsync(updatedUser.Id);

        // Update the user's properties
        user.FirstName = updatedUser.FirstName?.Trim();
        user.LastName = updatedUser.LastName?.Trim();
        user.Role = updatedUser.Role;
        user.Email = updatedUser.Email;
        user.EmailConfirmed = updatedUser.EmailConfirmed;
        user.PhoneNumber = updatedUser.PhoneNumber?.Trim();

        // Update the user in the database
        await _userManager.UpdateAsync(user);

        if (user.Role == "Admin" && !await IsInRoleAsync(user, "Admin"))
        {
            if (await IsInRoleAsync(user, "User"))
            {
                await RemoveRoleAsync(user, "User");
            }
            await AddRoleAsync(user, "Admin");
        }
        else if (user.Role == "User" && (await IsInRoleAsync(user, "Admin")))
        {
            if (await IsInRoleAsync(user, "Admin"))
            {
                await RemoveRoleAsync(user, "Admin");
            }
            await AddRoleAsync(user, "User");
        }
    }

    public ApplicationUser GetLoggedUser()
    {
        _context.DetachAllEntities();
        using Context context = _context.Clone();
        context.DetachAllEntities();
        ApplicationUser? user = context.Users.SingleOrDefault(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);

        return user;

    }




    public bool IsLoggedUserAdmin()
    {


        return _httpContextAccessor.HttpContext.User.IsInRole("Admin");




    }

    public async void Login(string username, string password, bool rememberMe)
    {
        Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            // handle failed login attempt
        }
    }
    public void Logout()
    {
        _ = _signInManager.SignOutAsync();
    }

    public async Task AddRoleAsync(ApplicationUser user, string role)
    {
        _ = await _userManager.AddToRoleAsync(user, role);
    }

    public async Task RemoveRoleAsync(ApplicationUser user, string role)
    {
        _ = await _userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<IList<string>> GetRoleAsync(ApplicationUser user)
    {

        return user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
    {
        return (await GetRoleAsync(user)).Contains(role);
    }

    public async Task<ApplicationUser> ToExistingApplicationUser(InputModel inputModel)
    {
        ApplicationUser applicationUser = await GetApplicationUserByIdAsync(inputModel.Id);
        if (inputModel.PhoneNumber != null)
        {
            applicationUser.PhoneNumber = inputModel.PhoneNumber.Trim();
        }
        if (inputModel.FirstName != null)
        {
            applicationUser.FirstName = inputModel.FirstName.Trim();
        }
        if (inputModel.LastName != null)
        {
            applicationUser.LastName = inputModel.LastName.Trim();
        }
        if (inputModel.Username != null)
        {
            applicationUser.UserName = inputModel.Username.Trim();
        }
        if (inputModel.Email != null)
        {
            applicationUser.Email = inputModel.Email.Trim();
        }
        if (inputModel.Role != null)
        {
            applicationUser.Role = inputModel.Role.Trim();
        }
        return applicationUser;
    }

    public ApplicationUser ToApplicationUser(InputModel inputModel)
    {
        ApplicationUser applicationUser = new ApplicationUser
        {
            Id = inputModel.Id,
            PhoneNumber = inputModel.PhoneNumber,
            FirstName = inputModel.FirstName,
            UserName = inputModel.Username,
            LastName = inputModel.LastName,
            Email = inputModel.Email
        };
        return applicationUser;
    }
}