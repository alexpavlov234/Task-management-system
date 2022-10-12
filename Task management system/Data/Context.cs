using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Cards;
using System.Reflection.Metadata;
using Task_management_system.Models;

namespace Task_management_system.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        //Създаване на таблици в реална база данни
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>();
            modelBuilder.Entity<Role>().HasData(new Role {RoleId= 1, RoleName = "Администратор", CreateProjectPermission = true, CreateTaskPermission = true, EditProjectPermission = true, EditTaskPermission = true, ManageMembersPermission = true }) ;
            modelBuilder.Entity<Status>();
            modelBuilder.Entity<Assignment>();
            modelBuilder.Entity<User>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
