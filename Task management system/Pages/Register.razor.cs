using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Pages
{
    public partial class Register
    {
        private SignInModel signInModel = new SignInModel()
        {
            UserName = "",
            UserFirstName = "",
            UserLastName = "",
            UserEmail = "",
            UserPassword = ""
        };
        public bool IsContinue { get; set; } = false;

        private string RepeatPass;
        private string error = string.Empty;

        private async Task RegisterUser()
        {
        }

       
    }
}
