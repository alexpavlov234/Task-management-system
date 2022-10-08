using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.PdfViewer;
using System.ComponentModel.DataAnnotations;
using Task_management_system.Data;
using Task_management_system.Models;

namespace Task_management_system.Pages
{
    public partial class Register
    {
        [Inject]
        private NavigationManager NavMgr { get; set; }

        [Inject]
        private Context context { get; set; }
        private User user = new User()
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
      

            context.Add<User>(user);


            context.SaveChanges();
        }
        private async Task NavToLogin()
        {
            NavMgr.NavigateTo($"/Login", true);
        }
    }
}
