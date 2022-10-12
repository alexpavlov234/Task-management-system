using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;

namespace Task_management_system.Pages
{
    public partial class Login
    {
        [Inject]
        private IUserService userService { get; set; } = default!;
        [Inject]
        private LoginState loginState { get; set; }
        [Inject]
        private NavigationManager NavMgr { get; set; }


        private SignInModel userLogin = new SignInModel() { UserName = "", UserEmail = "", UserPassword = "" };
        public string Email { get; set; }
        public bool IsContinue { get; set; } = false;

        private string password;
        private string error = string.Empty;

        private async Task LoginUser()
        {
            var user = userService.GetSingleUser(userLogin.UserName);

            if (user != null)
            {
                if (user.UserPassword.Equals(userLogin.UserPassword))
                {
                    loginState.SetLogin(true, userService.GetSingleUser(user.Username));
                }
            }

        }
        
        private async Task NavToRegister()
        {
            NavMgr.NavigateTo($"/Register", true);
        }
        private void clearFields()
        {
            this.userLogin = new SignInModel();
        }
    }


}

