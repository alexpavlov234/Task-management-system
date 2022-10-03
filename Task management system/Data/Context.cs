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


        public DbSet<Project> Projects { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
