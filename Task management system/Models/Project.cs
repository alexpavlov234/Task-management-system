using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Areas.Identity;
namespace Task_management_system.Models
{
    [Table("Project")]
    [Display(Name = "Проект")]
    public class Project
    {
        public Project()
        {
            ProjectParticipants = new List<ApplicationUserProject>();
        }
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Полето 'Име на проект' е задължително!")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Полето 'Име на проект' не може да бъде празно!")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage = "Полето 'Описание на проект' е задължително!")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Полето 'Описание на проекта' не може да бъде празно!")]
        public string ProjectDescription { get; set; }
        [Required(ErrorMessage = "Моля изберете начална дата!")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Моля изберете крайна дата!")]
        public DateTime EndDate { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Issue> Issues { get; set; }
        [Required(ErrorMessage = "Полето 'Собственик на проект' е задължително!")]
        [System.Text.Json.Serialization.JsonIgnore]
        public ApplicationUser ProjectOwner { get; set; }
        [Required(ErrorMessage = "Моля изберете поне един участник в проекта!"),]
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<ApplicationUserProject>? ProjectParticipants { get; set; }
        [Required(ErrorMessage = "Полето 'Тип на проект' е задължително!")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Полето 'Тип на проект' не може да бъде празно!")]
        public string ProjectType { get; set; }
    }
}
