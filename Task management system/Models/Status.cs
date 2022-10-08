using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_management_system.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        public int StatusId { get; set; }
        [Required]
        public string StatusName { get; set; }


    }
}
