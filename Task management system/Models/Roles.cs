using Microsoft.AspNetCore.Identity;

namespace Task_management_system.Models
{
    public class Roles
    {
        public RoleManager<IdentityRole> roleManager { get; set; }
        public List<Microsoft.AspNetCore.Identity.IdentityRole> Items;
        public Roles()
        {
            Items = roleManager.Roles.ToList();
        }


    }
}
