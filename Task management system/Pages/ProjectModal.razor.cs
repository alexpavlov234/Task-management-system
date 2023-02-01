using KeyValue_management_system.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Common;
using Task_management_system.Services.Common;

namespace Task_management_system.Pages
{
    public partial class ProjectModal
    {
        protected EditContext editContext;
        private Project project = new Project();
        private bool IsUserNew = false;
        private bool IsVisible = false;
        private DateTime MinDate = new DateTime(1900, 1, 1);
        private ToastMsg toast = new ToastMsg();
        private List<KeyValue> projectTypes { get; set; }
        private List<ApplicationUser> users { get; set; }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        private BaseHelper BaseHelper { get; set; }

        [Inject]
        private IKeyValueService keyValueService { get; set; }
        
        [Inject]
        private IUserService UserService { get; set; }


        protected async override Task OnInitializedAsync()
        {
            projectTypes = keyValueService.GetAllKeyValuesByKeyType("ProjectType");
            users = UserService.GetAllUsers();
        }
        public async void OpenDialog(Project project)
        {
            
            editContext = new EditContext(project);

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

                if (ProjectService.GetProjectById(project.ProjectId) != null)
                {

                    ProjectService.UpdateProject(project);
                    await CallbackAfterSubmit.InvokeAsync();
                    toast.sfSuccessToast.Title = "Успешно приложени промени!";
                    toast.sfSuccessToast.ShowAsync();
                    IsVisible = false;
                }
                else
                {
                    string result = ProjectService.CreateProject(project);
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

                }
            }
        }


    }
}