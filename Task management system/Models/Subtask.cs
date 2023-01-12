using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_management_system.Models
{
    [Table("Subtask")]
    [Display(Name = "Подзадача")]
    public class Subtask
    {
        [Key]
        public int SubtaskId { get; set; }

        [Required]
        public string SubtaskName { get; set; }

        [Required]
        public string SubtaskDescription { get; set; }

        [Required]
        public DateTime SubtaskLastEditedDate { get; set; }

        [Required]
        public DateTime SubtaskCompletionDate { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Status { get; set; }

        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public Nullable<int> RecurrenceID { get; set; }

      
    }
}