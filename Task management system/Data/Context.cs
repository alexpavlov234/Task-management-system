using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Cards;
using System.Reflection.Metadata;
using Task_management_system.Areas.Identity;
using Task_management_system.Models;

namespace Task_management_system.Data
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        //Създаване на таблици в реална база данни
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasIndex(b => b.ProjectName)
            .IsUnique();
            modelBuilder.Entity<Models.Task>();
            modelBuilder.Entity<Subtask>();
            modelBuilder.Entity<KeyType>();
            modelBuilder.Entity<KeyValue>();
              
            // modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<KeyType> KeyType { get; set; }
        public DbSet<KeyValue> KeyValue { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Models.Subtask> Subtasks { get; set; }

    }
}
