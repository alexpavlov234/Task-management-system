using Microsoft.EntityFrameworkCore;
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

        public string AssigneeId { get; set; }
        public ApplicationUser Assignee { get; set; }

        public string AssignedТoId { get; set; }
        public ApplicationUser AssignedТo { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        public string Subject { get; set; }

        public string Location { get; set; }

        [Required(ErrorMessage = "Полето 'Статус' е задължително!")]
        public string Status { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Полето 'Проект' е задължително!")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Полето 'Приоритет' е задължително!")]
        public string Priority { get; set; }
        public Project Project { get; set; }

        
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public Nullable<int> RecurrenceID { get; set; }
        [Required]
        public ICollection<Subtask> Subtasks { get; set; }
    }
}