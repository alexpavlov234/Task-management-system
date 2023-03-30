using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Task_management_system.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class UnlockAccountModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UnlockAccountModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Неуспешно зареждане на потребител с ID '{userId}'.");
            }

            var result = await _userManager.VerifyUserTokenAsync(user, "Default", "UnlockAccount", token);
            if (!result)
            {
                return BadRequest("Невалиден токен.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);

            return Page();
        }
    }
}
