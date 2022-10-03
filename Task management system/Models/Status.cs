using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }
        [Required]
        public string StatusName { get; set; }


    }
}
