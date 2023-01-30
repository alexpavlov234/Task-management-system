using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Areas.Identity;

namespace Task_management_system.Models
{
    [Table("Task")]
    [Display(Name = "Задача")]
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public string TaskName { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        public ApplicationUser Assignee { get; set; }


        public ApplicationUser AssignedТo { get; set; }




        [Required]
        public DateTime TaskLastEditedDate { get; set; }

        [Required]
        public DateTime TaskCompletionDate { get; set; }

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