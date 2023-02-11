using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Areas.Identity;

namespace Task_management_system.Models
{
    [Table("Project")]
    [Display(Name = "Проект")]
    [Index(nameof(ProjectName), IsUnique = true)]
    public class Project
    {
        public Project()
        {
            this.ProjectParticipants = new List<ApplicationUserProject>();
        }
        [Key]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Полето 'Име на проект' е задължително!")]
         public string ProjectName { get; set; }
        [Required(ErrorMessage = "Полето 'Описание на проект' е задължително!")]
        public string ProjectDescription { get; set; }

        [Required(ErrorMessage = "Моля изберете начална дата!")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Моля изберете крайна дата!")]
        public DateTime EndDate { get; set; }


        public ICollection<Issue> Tasks { get; set; }
        [Required(ErrorMessage = "Полето 'Собственик на проект' е задължително!")]
        public ApplicationUser ProjectOwner { get; set; }
        [Required(ErrorMessage = "Моля изберете поне един участник в проекта!")]
        public IEnumerable<ApplicationUserProject>? ProjectParticipants { get; set; }

        
       
        [Required(ErrorMessage = "Полето 'Тип на проект' е задължително!")]
        [ForeignKey(nameof(KeyValue))]
        public int IdProjectType { get; set; }
        public KeyValue ProjectType { get; set; }
    }
}
