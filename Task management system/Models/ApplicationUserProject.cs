using Task_management_system.Areas.Identity;
namespace Task_management_system.Models
{
    public class ApplicationUserProject
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
