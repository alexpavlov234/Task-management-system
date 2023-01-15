using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
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

    public void CreateApplicationUser(ApplicationUser applicationUser, String Password)
    {
        _userManager.CreateAsync(applicationUser, Password);
    }

    public void DeleteApplicationUser(ApplicationUser applicationUser)
    {
        _userManager.DeleteAsync(applicationUser);
    }

    public List<ApplicationUser> GetAllUsers()
    {
        return _userManager.Users.ToList();
    }

    public ApplicationUser? GetApplicationUserByIdAsync(string Id)
    {
        var user = _context.Users.Where<ApplicationUser>(x => x.Id == Id).FirstOrDefault();
        return user;
    }

    public ApplicationUser? GetApplicationUserByUsernameAsync(string Username)
    {
        var user = _context.Users.Where<ApplicationUser>(x => x.UserName == Username).FirstOrDefault();
        return user;
    }
    public void UpdateApplicationUser(ApplicationUser applicationUser)
    {
        _userManager.UpdateAsync(applicationUser);
    }

    public ApplicationUser? GetLoggedUser()
    {
        return _context.Users.Single(predicate: r => r.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);
    }

    public bool IsUserLoggedIn() => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

    public async Task<bool> IsLoggedUserAdmin()
    {
        if (IsUserLoggedIn())
        {
            return await _userManager.IsInRoleAsync(GetLoggedUser(), "Admin");
        }
        else
        {
            return false;
        }
    }
}