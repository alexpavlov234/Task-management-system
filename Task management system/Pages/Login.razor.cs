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
        private NavigationManager NavMgr { get; set; }


        private SignInModel userLogin = new SignInModel() { UserName = "", UserEmail = "", UserPassword = "" };
        public string Email { get; set; }
        public bool IsContinue { get; set; } = false;

        private string password;
        private string error = string.Empty;

        private async Task LoginUser()
        {
            

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

