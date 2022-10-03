namespace Task_management_system.Pages
{
    public partial class Login
    {
        private SignInModel signInModel = new SignInModel() { UserName = "", Email = "", Password = "" };
        public string Email { get; set; }
        public bool IsContinue { get; set; } = false;

        private string password;
        private string error = string.Empty;

        private async Task RegisterUser()
        {
        }

        private async Task ForgotPassword()
        {
            //NavMgr.NavigateTo($"/ForgotPassword", true);
        }
    }
}