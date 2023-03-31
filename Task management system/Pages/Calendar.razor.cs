using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Blazor.Schedule;
using Task_management_system.Areas.Identity;
using Task_management_system.Interfaces;
using Task_management_system.Models;
using Task_management_system.Pages.Issues;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages
{
    public partial class Calendar
    {
        [Inject]
        private RoleManager<IdentityRole> RoleManager { get; set; }
        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; }
        [Inject]
        private SignInManager<ApplicationUser> SignInManager { get; set; }
        [Inject]
        private IKeyValueService KeyValueService { get; set; }
        [Inject]
        private IProjectService ProjectService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IIssueService IssueService { get; set; }

        readonly DateTime CurrentDate = DateTime.Now;
        private List<Issue> issues { get; set; }
        private ToastMsg toast = new ToastMsg();
        private IssueModal issueModal = new IssueModal();
        public void OnResized(ResizeEventArgs<Issue> args)
        {
            args.Cancel = true;
        }
        public async Task OnDragged(DragEventArgs<Issue> args)
        {
            args.Cancel = true;
        }
        protected async override Task OnInitializedAsync()
        {
            issues = IssueService.GetAllIssues(UserService.GetLoggedUser().Id);
        }
        public void OnPopupOpen(PopupOpenEventArgs<Issue> args)
        {
            args.Cancel = true;
            issueModal.OpenDialog(args.Data);
        }
        private async Task UpdateAfterIssueModalSubmitAsync()
        {
            issues = IssueService.GetAllIssues(UserService.GetLoggedUser().Id);
            StateHasChanged();
        }
    }
}
