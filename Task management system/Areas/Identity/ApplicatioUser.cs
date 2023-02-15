using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Models;

namespace Task_management_system.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            ProjectsParticipants = new List<ApplicationUserProject>();
            ProjectsOwners = new List<Project>();
            AssignToUsers = new List<Issue>();
            AssigneeUsers = new List<Issue>();
        }

        [Required(ErrorMessage = "Полето 'Име' е задължително")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Полето 'Фамилия' е задължително")]
        public string LastName { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Issue> AssigneeUsers { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Issue> AssignToUsers { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Project> ProjectsOwners { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual IEnumerable<ApplicationUserProject>? ProjectsParticipants { get; set; }

        [NotMapped]
        public string Role { get; set; }

        [NotMapped]

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
