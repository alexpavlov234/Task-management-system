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

        [Required]
        public string IssueName { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        public ApplicationUser Assignee { get; set; }


        public ApplicationUser AssignedТo { get; set; }




        [Required]
        public DateTime IssueLastEditedDate { get; set; }

        [Required]
        public DateTime IssueCompletionDate { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }



        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public Nullable<int> RecurrenceID { get; set; }
        [Required]
        public ICollection<Subtask> Subtasks { get; set; }
    }
}