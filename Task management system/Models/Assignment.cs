using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class Assignment
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public string TaskName { get; set; }
        [Required]
        public string TaskDescription { get; set; }

        [Required]
        public DateTime TaskCreationDate { get; set; }

        [Required]
        public DateTime TaskEndDate { get; set; }

        [Required]
        public DateTime TaskLastEditedDate { get; set; } 
        [Required]
        public DateTime TaskCompletionDate { get; set; }

        public int UserId { get; set; }
        public User TaskLeader { get; set; }
    }
}
