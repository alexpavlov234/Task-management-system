using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.PdfViewer;
using System.ComponentModel.DataAnnotations;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Services;

namespace Task_management_system.Pages
{
    public partial class Register
    {
        [Inject]
        private NavigationManager NavMgr { get; set; }

        [Inject]
        private IUserService userService { get; set; } = default!;

        [Inject]
        private Context context { get; set; }
        private User user;
        public bool IsContinue { get; set; } = false;

        private string RepeatPass;
        private string error = string.Empty;

        private async Task RegisterUser()
        {
            //user.UserRole = context.Find<Role>(1);
            //userService.InsertUser(user);
   
        }
        private async Task NavToLogin()
        {
            NavMgr.NavigateTo($"/Login", true);
        }
    }
}
