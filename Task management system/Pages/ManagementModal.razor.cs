using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Task_management_system.Areas.Identity;
using Task_management_system.Pages.Common;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages
{
    public partial class ManagementModal
    {
        protected EditContext editContext;
        private InputModel inputModel = new InputModel();
        private bool IsUserNew = false;
        private bool IsVisible = false;

        private List<Role> Roles = new List<Role> {
            new Role() { ID= "Admin", Text= "Администратор" },
             new Role() { ID= "User", Text= "Потребител" }
        };

        private ToastMsg toast = new ToastMsg();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        private BaseHelper BaseHelper { get; set; }

        public void GeneratePassword()
        {
            inputModel.Password = BaseHelper.GeneratePassword();
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
            IsUserNew = applicationUser != null ? (await UserService.GetApplicationUserByIdAsync(applicationUser.Id)) == null : false;
            editContext = new EditContext(inputModel);
            if (IsUserNew) { GeneratePassword(); }
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
                ApplicationUser User = await UserService.ToApplicationUser(inputModel);

                if ((await UserService.GetApplicationUserByIdAsync(User.Id)) != null)
                {
                    await UserService.UpdateApplicationUser(User);
                    await CallbackAfterSubmit.InvokeAsync();
                    toast.sfSuccessToast.Title = "Успешно приложени промени!";
                    toast.sfSuccessToast.ShowAsync();
                    IsVisible = false;
                }
                else
                {
                    string result = await UserService.CreateApplicationUser(User, inputModel.Password);
                    if (result.StartsWith("Успешно"))
                    {
                        toast.sfSuccessToast.Title = result;
                        toast.sfSuccessToast.ShowAsync();
                    }
                    else
                    {
                        toast.sfErrorToast.Title = result;
                        toast.sfErrorToast.ShowAsync();
                    }

                    await CallbackAfterSubmit.InvokeAsync();
                    IsVisible = false;
                }
            }
        }

        public class Role
        {
            public string ID { get; set; }
            public string Text { get; set; }
        }
    }
}