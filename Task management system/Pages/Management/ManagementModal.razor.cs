using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Shared;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages.Management
{
    public partial class ManagementModal
    {
        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private BaseHelper BaseHelper { get; set; }
        protected EditContext editContext;
        private readonly InputModel inputModel = new InputModel();
        private bool IsUserNew = false;
        private bool IsVisible = false;
        private readonly List<Role> Roles = new List<Role> {
            new Role() { ID= "Admin", Text= "Администратор" },
            new Role() { ID= "User", Text= "Потребител" }
        };
        private ToastMsg toast = new ToastMsg();
        public class Role
        {
            public string ID { get; set; }
            public string Text { get; set; }
        }
        public void GeneratePassword()
        {
            inputModel.Password = BaseHelper.GeneratePassword();
            inputModel.ConfirmPassword = inputModel.Password;
        }
        public async void OpenDialog(ApplicationUser applicationUser)
        {
            inputModel.Id = applicationUser.Id;
            inputModel.FirstName = applicationUser.FirstName;
            inputModel.LastName = applicationUser.LastName;
            inputModel.Username = applicationUser.UserName;
            inputModel.Email = applicationUser.Email;
            inputModel.Role = applicationUser.Role;
            inputModel.PhoneNumber = applicationUser.PhoneNumber;
            IsUserNew = applicationUser != null && (await UserService.GetApplicationUserByIdAsync(applicationUser.Id)) == null;
            editContext = new EditContext(inputModel);
            if (IsUserNew) { GeneratePassword(); }
            else
            {
                IList<string> roles = await UserService.GetRoleAsync(applicationUser);
                if (roles.Contains("Admin"))
                {
                    inputModel.Role = "Admin";
                }
                else
                {
                    inputModel.Role = "User";
                }
            }
            IsVisible = true;
            StateHasChanged();
        }
        private void CloseDialog()
        {
            IsVisible = false;
            StateHasChanged();
        }
        private async void SaveUser()
        {
            if (editContext.Validate())
            {
                if ((await UserService.GetApplicationUserByIdAsync(inputModel.Id)) != null)
                {
                    ApplicationUser User = await UserService.ToExistingApplicationUser(inputModel);
                    await UserService.UpdateApplicationUser(User);
                    await CallbackAfterSubmit.InvokeAsync();
                    toast.sfSuccessToast.Title = "Успешно приложени промени!";
                    _ = toast.sfSuccessToast.ShowAsync();
                }
                else
                {
                    string result = await UserService.CreateApplicationUser(UserService.ToApplicationUser(inputModel), inputModel.Password);
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        _ = toast.sfSuccessToast.ShowAsync();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        _ = toast.sfErrorToast.ShowAsync();
                    }
                    await CallbackAfterSubmit.InvokeAsync();
                }
            }
        }
    }
}