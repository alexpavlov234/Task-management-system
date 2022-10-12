using Microsoft.AspNetCore.Components;

namespace Task_management_system.Pages
{
    public partial class Index
    {
        [Inject]
        private NavigationManager NavMgr { get; set; }

        private async Task NavToLogin()
        {
            NavMgr.NavigateTo($"/Login", true);
        }
        
    }
}
