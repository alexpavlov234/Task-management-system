using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Task_management_system.Models;

namespace Task_management_system.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
