using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string UserName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string UserFirstName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string UserLastName { get; set; }

        [Required]
        ///TODO: Email Validation
        public string UserEmail { get; set; }

        [Required]
        ///TODO: Pass Validation
        public string UserPassword { get; set; }


        public int RoleId { get; set; }
        public Role Role { get; set; }


    }
}
