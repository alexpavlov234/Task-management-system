using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Models;

namespace Task_management_system.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Полето 'Име' е задължително")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Полето 'Фамилия' е задължително")]
        public string LastName { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
