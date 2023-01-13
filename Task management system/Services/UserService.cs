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
    public UserService(Context context, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _emailSender = emailSender;
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
        _userManager.UpdateAsync(applicationUser);s
    }
}