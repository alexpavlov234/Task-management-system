using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Cards;
using System.Reflection.Metadata;
using Task_management_system.Models;

namespace Task_management_system.Data
{
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        //Създаване на таблици в реална база данни
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>();
            modelBuilder.Entity<Status>();
            modelBuilder.Entity<Assignment>();
           // modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Status> Statuses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

    }
}
