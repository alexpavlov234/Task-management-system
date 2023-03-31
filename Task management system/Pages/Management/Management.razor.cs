using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Services.Common;
namespace Task_management_system.Pages.Management
{
    public partial class Management
    {
        [Parameter]
        public bool DisableWhenProcedureIsCompleted { get; set; }
        [Parameter]
        public bool DisableAllFields { get; set; }
        [Inject]
        public RoleManager<IdentityRole> RoleManager { get; set; }
        [Inject]
        public UserManager<ApplicationUser> UserManager { get; set; }
        [Inject]
        public SignInManager<ApplicationUser> SignInManager { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public Context context { get; set; }
        [Inject]
        public IHttpContextAccessor httpContextAccessor { get; set; }
        private readonly bool documentDeleteConfirmed = false;
        private readonly bool showDocumentConfirmDialog = false;
        private ManagementModal managementModal = new ManagementModal();
        private SfGrid<ApplicationUser> usersGrid = new SfGrid<ApplicationUser>();
        private ToastMsg toast = new ToastMsg();
        private readonly Dictionary<string, string> userRoles = new Dictionary<string, string>();
        public int RowCounter = 0;
        public List<IdentityRole> roles { get; set; }
        public List<ApplicationUser> users { get; set; }
        SfGrid<ApplicationUser> sfGrid { get; set; }
        private async Task UpdateData()
        {
            List<ApplicationUser> users = UserService.GetAllUsers();
            bool itemToRemove = users.Remove(users.Single(r => r.UserName == httpContextAccessor.HttpContext.User.Identity.Name));
            List<IdentityRole> roles = RoleManager.Roles.ToList();
            this.users = users;
            this.roles = roles;
            userRoles.Clear();
            foreach (ApplicationUser user in this.users)
            {
                userRoles.Add(user.UserName, (await UserService.GetRoleAsync(user)).Contains("Admin") ? "Администратор" : "Потребител");
            }
        }
        private async Task UpdateAfterManagementModalSubmitAsync()
        {
            await UpdateData();
            await usersGrid.Refresh();
            StateHasChanged();
        }
        private async Task AddApplicationUser()
        {
            managementModal.OpenDialog(new ApplicationUser());
        }
        private async Task DeleteUser(ApplicationUser applicationUser)
        {
            string result = await UserService.DeleteApplicationUser(applicationUser);
            List<ApplicationUser> users = UserService.GetAllUsers();
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
            bool itemToRemove = users.Remove(users.Single(r => r.UserName == httpContextAccessor.HttpContext.User.Identity.Name));
            List<IdentityRole> roles = RoleManager.Roles.ToList();
            this.users = users;
            this.roles = roles;
            StateHasChanged();
        }
        private async Task EditUser(ApplicationUser applicationUser)
        {
            managementModal.OpenDialog(applicationUser);
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "usersGrid_pdfexport")
            {
                int temp = usersGrid.PageSettings.PageSize;
                usersGrid.PageSettings.PageSize = users.Count();
                await usersGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>
                {
#pragma warning disable BL0005
                    new GridColumn() { Field = nameof(ApplicationUser.UserName), HeaderText = "Потребителско име", Width = "80", TextAlign = TextAlign.Left },
                    new GridColumn() { Field = nameof(ApplicationUser.FirstName), HeaderText = "Име", Width = "80", TextAlign = TextAlign.Left },
                    new GridColumn() { Field = nameof(ApplicationUser.LastName), HeaderText = "Фамилия", Width = "80", TextAlign = TextAlign.Left },
                    new GridColumn() { Field = nameof(ApplicationUser.Email), HeaderText = "Имейл", Width = "80", TextAlign = TextAlign.Left }
                };
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;
                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"Потребители.pdf";
                usersGrid.PageSettings.PageSize = temp;
                await usersGrid.Refresh();
                await usersGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id == "usersGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>
                {
#pragma warning disable BL0005
                    new GridColumn() { Field = nameof(ApplicationUser.UserName), HeaderText = "Потребителско име", Width = "80", TextAlign = TextAlign.Left },
                    new GridColumn() { Field = nameof(ApplicationUser.FirstName), HeaderText = "Име", Width = "80", TextAlign = TextAlign.Left },
                    new GridColumn() { Field = nameof(ApplicationUser.LastName), HeaderText = "Фамилия", Width = "80", TextAlign = TextAlign.Left },
                    new GridColumn() { Field = nameof(ApplicationUser.Email), HeaderText = "Имейл", Width = "80", TextAlign = TextAlign.Left }
                };
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Потребители.xlsx";
                await usersGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        protected async override Task OnInitializedAsync()
        {
            await UpdateData();
        }
        public async Task<int> GetRowCounter(IdentityRole val)
        {
            RowCounter = roles.IndexOf(val);
            return RowCounter + 1;
        }
    }
}
