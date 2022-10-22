using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class RegisterViewModel
    {

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Потвърждаване на паролата")]
        [Compare ("Password", ErrorMessage = "Паролите не съвпадат!")]

        public string ConfirmPassword { get; set; }
    }
}
