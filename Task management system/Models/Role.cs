using Syncfusion.Blazor.TreeGrid.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_management_system.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Прекалено дълъг низ!")]
        public string RoleName { get; set; }
        [Required]
        public bool CreateProjectPermission { get; set; }
        [Required]
        public bool EditProjectPermission { get; set; }
        [Required]
        public bool ManageMembersPermission { get; set; }
        [Required]
        public bool CreateTaskPermission { get; set; }
        [Required]
        public bool EditTaskPermission { get; set; }

        public List<User> Users { get; set; }




    }
}
