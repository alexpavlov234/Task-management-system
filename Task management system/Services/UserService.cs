using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor.Inputs;
using System;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;

public class UserService : Controller, IUserService
{
    private readonly Context _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IEmailSender _emailSender;
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

    public ApplicationUser? GetApplicationUserByUsernameAsync(string Username)
    {
        var user = _context.Users.Where<ApplicationUser>(x => x.UserName == Username).FirstOrDefault();
        return user;
    }
    public ApplicationUser? GetApplicationUserByIdAsync(string Id)
    {
        var user = _context.Users.Where<ApplicationUser>(x => x.Id == Id).FirstOrDefault();
        return user;
    }

    public void CreateApplicationUser(ApplicationUser applicationUser, String Password)
    {
        _userManager.CreateAsync(applicationUser, Password);
    }

    public void UpdateApplicationUser(ApplicationUser applicationUser)
    {
        _userManager.UpdateAsync(applicationUser);
    }

    public void DeleteApplicationUser(ApplicationUser applicationUser)
    {
        _userManager.DeleteAsync(applicationUser);
        
    }

    public IQueryable<ApplicationUser> GetAllUsers()
    {
        return _userManager.Users;
    }
    
}
