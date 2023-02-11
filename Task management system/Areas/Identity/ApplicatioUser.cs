using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_management_system.Models;

namespace Task_management_system.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.ProjectsParticipants = new List<ApplicationUserProject>();
            this.ProjectsOwners = new List<Project>();
            this.AssignToUsers = new List<Issue>();
            this.AssigneeUsers = new List<Issue>();
        }

        [Required(ErrorMessage = "Полето 'Име' е задължително")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Полето 'Фамилия' е задължително")]
        public string LastName { get; set; }

        public virtual ICollection<Issue> AssigneeUsers { get; set; } 
        public virtual ICollection<Issue> AssignToUsers { get; set; } 
        public virtual ICollection<Project> ProjectsOwners { get; set; } 
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
