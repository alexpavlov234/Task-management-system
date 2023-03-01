using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class InputModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Полето 'Потребителско име' е задължително!")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Потребителското име не трябва да съдържа интервали!")]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [RegularExpression(@"^[A-ZА-ЯЁ][A-Za-zА-ЯЁа-яё]+-?[A-Za-zА-ЯЁа-яё]+$", ErrorMessage = "Невалидно име!")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [RegularExpression(@"^[A-ZА-ЯЁ][A-Za-zА-ЯЁа-яё]+-?[A-Za-zА-ЯЁа-яё]+$", ErrorMessage = "Невалидно фамилно име!")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Полето 'Имейл' е задължително!")]
        [EmailAddress(ErrorMessage = "Имейлът е невалиден!")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }



        [StringLength(100, ErrorMessage = "Дължината на паролата трябва да бъде най-малко {2} и най-много {1} символа!", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }



        [Display(Name = "Потвърждаване на паролата")]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат!")]
        public string ConfirmPassword { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон")]
        public virtual string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
