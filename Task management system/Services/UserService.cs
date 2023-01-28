using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;

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
    public async Task<string> CreateApplicationUser(ApplicationUser applicationUser, String Password)
    {
        var result = await _userManager.CreateAsync(applicationUser, Password);
        _context.Entry(applicationUser).State = EntityState.Detached;
        if (!result.Succeeded)
        {
            return result.Errors.ToList().First().Description;
        }
        else
        {
            return "Успешно създаден потребител!";
        }
    }

    public async Task DeleteApplicationUser(ApplicationUser applicationUser)
    {
        var local = _context.Set<ApplicationUser>()
    .Local
    .FirstOrDefault(entry => entry.Id.Equals(applicationUser.Id));

        // check if local is not null 
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        // set Modified flag in your entry
        _context.Entry(applicationUser).State = EntityState.Deleted;

        // save 
        _context.SaveChanges();

    }

    public List<ApplicationUser> GetAllUsers()
    {
        return _context.Users.ToList();

    }

    public async Task<ApplicationUser> GetApplicationUserByIdAsync(string Id)
    {
        var user = await _userManager.FindByIdAsync(Id);
        return user;
    }

    public async Task<ApplicationUser> GetApplicationUserByUsernameAsync(string Username)
    {
        var user = await _userManager.FindByNameAsync(Username);
        return user;
    }

    public async Task UpdateApplicationUser(ApplicationUser applicationUser)
    {
        var local = _context.Set<ApplicationUser>().Local.FirstOrDefault(entry => entry.Id.Equals(applicationUser.Id));
        // check if local is not null
        if (local != null)
        {
            // detach
            _context.Entry(local).State = EntityState.Detached;
        }
        _context.Entry(applicationUser).State = EntityState.Modified;
        _context.SaveChanges();

    }


    public async Task<ApplicationUser> GetLoggedUser()
    {
        var user = await _userManager.GetUserAsync(_signInManager.Context.User);
        _context.Entry(user).State = EntityState.Detached;
        return user;

    }



    public async Task<bool> IsLoggedUserAdmin()
    {

        return await _userManager.IsInRoleAsync(await GetLoggedUser(), "Admin");



    }

    public async void Login(string username, string password, bool rememberMe)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: true);
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


}