using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Areas.Identity;

namespace Task_management_system.Models
{
    [Table("Issue")]
    [Display(Name = "Задача")]
    public class Issue
    {
        [Key]
        public int IssueId { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public ApplicationUser Assignee { get; set; }

        public ApplicationUser AssignedТo { get; set; }

        [Required]
        public DateTime LastEditedDate { get; set; }
        
        [Required]
        public string Subject { get; set; }

        public string Location { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public Nullable<int> RecurrenceID { get; set; }
      
        public ICollection<Subtask> Subtasks { get; set; }

        
    }
}