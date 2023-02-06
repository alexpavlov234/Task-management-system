using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Models;

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
            await _userManager.AddToRoleAsync(applicationUser, "User");
            // Успешно създаден потребител
            return "Успешно създаден потребител!";
        }
    }

    // Изтриване на потребител
    public async Task DeleteApplicationUser(ApplicationUser applicationUser)
    {
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
        _context.SaveChanges();

    }







    // Връща списък с всички потребители
    public List<ApplicationUser> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    // Връща един потребител по зададен Id
    public async Task<ApplicationUser> GetApplicationUserByIdAsync(string Id)
    {
        // Търси в локалната база данни за потребител с зададеното Id
        ApplicationUser? local = _context.Users.AsNoTracking().FirstOrDefault(entry => entry.Id.Equals(Id));

        return local;
    }

    public async Task<ApplicationUser> GetApplicationUserByUsernameAsync(string Username)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(Username);
        return user;
    }

    public async Task UpdateApplicationUser(ApplicationUser applicationUser)
    {
        ApplicationUser? local = _context.Set<ApplicationUser>().Local.FirstOrDefault(entry => entry.Id.Equals(applicationUser.Id));
        // check if local is not null
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(applicationUser).State = EntityState.Modified;

        _context.SaveChanges();

        if (applicationUser.Role == "Admin" && !(await IsInRoleAsync(applicationUser, "Admin")))
        {
            if (await IsInRoleAsync(applicationUser, "User"))
            {
                await RemoveRoleAsync(applicationUser, "User");
            }
            await AddRoleAsync(applicationUser, "Admin");
        }
        else if (applicationUser.Role == "User" && (await IsInRoleAsync(applicationUser, "Admin")))
        {
            if (await IsInRoleAsync(applicationUser, "Admin"))
            {
                await RemoveRoleAsync(applicationUser, "Admin");
            }
            await AddRoleAsync(applicationUser, "User");
        }
    }


    public async Task<ApplicationUser> GetLoggedUser()
    {
        using (Context context = _context.Clone())
        {
            return await context.Users.SingleOrDefaultAsync(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);
        }

    }




    public async Task<bool> IsLoggedUserAdmin()
    {
        return await _userManager.IsInRoleAsync(await GetLoggedUser(), "Admin");
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
        _signInManager.SignOutAsync();
    }

    public async Task AddRoleAsync(ApplicationUser user, string role)
    {
        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task RemoveRoleAsync(ApplicationUser user, string role)
    {
        await _userManager.RemoveFromRoleAsync(user, role);
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
        applicationUser.PhoneNumber = inputModel.PhoneNumber;
        applicationUser.FirstName = inputModel.FirstName;
        applicationUser.LastName = inputModel.LastName;
        applicationUser.UserName = inputModel.Username;
        applicationUser.Email = inputModel.Email;
        applicationUser.Role = inputModel.Role;
        return applicationUser;
    }

    public ApplicationUser ToApplicationUser(InputModel inputModel)
    {
        ApplicationUser applicationUser = new ApplicationUser();
        applicationUser.Id = inputModel.Id;
        applicationUser.PhoneNumber = inputModel.PhoneNumber;
        applicationUser.FirstName = inputModel.FirstName;
        applicationUser.UserName = inputModel.Username;
        applicationUser.LastName = inputModel.LastName;
        applicationUser.Email = inputModel.Email;
        return applicationUser;
    }
}