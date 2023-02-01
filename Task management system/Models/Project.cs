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
        [Key]
        public int ProjectId { get; set; }

        [Required]

        public string ProjectName { get; set; }
        [Required]
        public string ProjectDescription { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }


        public ICollection<Issue> Tasks { get; set; }

        public ApplicationUser ProjectOwner { get; set; }

        public ICollection<ApplicationUser> ProjectParticipants { get; set; }

        public KeyValue ProjectType { get; set; }
    }
}
