using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
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
            //var user = await userManager.FindByNameAsync(userLogin.UserName);
          
            //if (user == null)
            //{
            //    error = "Грешно потребителско име.";
            //    clearFields();
            //    return;
            //}
            

            //if (await signInManager.CanSignInAsync(user))
            //{
            //    var result = await signInManager.CheckPasswordSignInAsync(user, userLogin.UserPassword, true);
            //    if (result == Microsoft.AspNetCore.Identity.SignInResult.Success)
            //    {
                    
                    
            //    }
            //    else
            //    {
            //        error = "Грешна парола.";
            //        clearFields();
            //    }
            //}
            //else
            //{
            //    error = "Несъществуващ акаунт!";
            //    clearFields();
            //}
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

