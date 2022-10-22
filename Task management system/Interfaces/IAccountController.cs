using Microsoft.AspNetCore.Mvc;
using Task_management_system.Models;

namespace Task_management_system.Interfaces
{
    public interface IAccountController
    {
        IActionResult Register();
        Task<IActionResult> Register(RegisterViewModel registerViewModel);
    }
}