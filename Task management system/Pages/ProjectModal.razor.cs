using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

        private ToastMsg toast = new ToastMsg();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        private BaseHelper BaseHelper { get; set; }



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