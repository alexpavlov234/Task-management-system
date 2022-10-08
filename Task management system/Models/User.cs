using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Полето \"Потребителско име\" е задължително!")]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string UserName { get; set; }
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


        public int RoleId { get; set; }
        public Role Role { get; set; }


    }
}
