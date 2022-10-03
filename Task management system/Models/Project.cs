using System.ComponentModel.DataAnnotations;

namespace Task_management_system.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string ProjectDescription { get; set; }

        [Required]
        public DateTime ProjectCreationDate { get; set; }

        [Required]
        public DateTime ProjectEndDate { get; set; }


        public int UserId { get; set; }
        public User ProjectLeader { get; set; }

        public ICollection<User> ProjectUsers { get; set; }


    }
}
