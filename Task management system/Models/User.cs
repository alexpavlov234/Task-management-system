using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_management_system.Models
{
    [Table("User")]
    [Index(nameof(User.Username), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Полето \"Потребителско име\" е задължително!")]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Полето \"Име\" е задължително!")]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string UserFirstName { get; set; }
        [Required(ErrorMessage = "Полето \"Фамилия\" е задължително!")]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string UserLastName { get; set; }

        [Required(ErrorMessage = "Полето \"Имейл\" е задължително!")]
        ///TODO: Email Validation
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Полето \"Парола\" е задължително!")]
        ///TODO: Pass Validation
        public string UserPassword { get; set; }
        
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role UserRole { get; set; }


    }
}
