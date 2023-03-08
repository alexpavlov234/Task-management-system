using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Areas.Identity;
using Task_management_system.Models;

namespace Task_management_system.Data
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        private readonly DbContextOptions<Context> _options;
        public Context(DbContextOptions<Context> options) : base(options)
        {
            _options = options;

        }
        //Създаване на таблици в реална база данни
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Issue>()
               .HasOne(i => i.Project)
               .WithMany(p => p.Issues)
               .HasForeignKey(i => i.ProjectId)
               .OnDelete(DeleteBehavior.NoAction);
            _ = modelBuilder.Entity<Issue>()
                .HasOne(i => i.Assignee)
                .WithMany(u => u.AssigneeUsers)
                .HasForeignKey(i => i.AssigneeId)
                .OnDelete(DeleteBehavior.NoAction);
            _ = modelBuilder.Entity<Issue>()
                .HasOne(i => i.AssignedТo)
                .WithMany(p => p.AssignToUsers)
                .HasForeignKey(i => i.AssignedТoId)
                .OnDelete(DeleteBehavior.NoAction);
            // modelBuilder.Entity<Issue>().HasOne(x => x.Assignee).WithMany(y => y.AssigneeUsers).OnDelete(DeleteBehavior.Cascade); 
            // modelBuilder.Entity<Issue>().HasOne(x => x.AssignedТo).WithMany(y => y.AssignToUsers).OnDelete(DeleteBehavior.Cascade);
            _ = modelBuilder.Entity<Project>().HasOne(x => x.ProjectOwner).WithMany(y => y.ProjectsOwners).OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<Project>().HasMany(x => x.ProjectsParticipants).WithMany(y => y.ProjectsParticipants).UsingEntity<ApplicationUserProject>("ApplicationUserProject"); 
            //modelBuilder.Entity<ApplicationUserProject>().HasKey(sc => new { sc.ProjectId, sc.UserId});
            _ = modelBuilder.Entity<ApplicationUserProject>()
                .HasKey(aup => new { aup.UserId, aup.ProjectId });
            _ = modelBuilder.Entity<ApplicationUserProject>()
                .HasOne(aup => aup.User)
                .WithMany(u => u.ProjectsParticipants)
                .HasForeignKey(aup => aup.UserId).OnDelete(DeleteBehavior.NoAction);
            _ = modelBuilder.Entity<ApplicationUserProject>()
                .HasOne(aup => aup.Project)
                .WithMany(p => p.ProjectParticipants)
                .HasForeignKey(aup => aup.ProjectId).OnDelete(DeleteBehavior.NoAction);

            _ = modelBuilder.Entity<Subtask>().HasOne(x => x.Issue).WithMany(i => i.Subtasks).HasForeignKey(x => x.IssueId).OnDelete(DeleteBehavior.Cascade);
            _ = modelBuilder.Entity<KeyType>();
            _ = modelBuilder.Entity<KeyValue>().HasIndex(b => b.KeyValueIntCode)
            .IsUnique();

            // modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.EnableSensitiveDataLogging();
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ApplicationUserProject> ApplicationUserProjects { get; set; }
        public DbSet<KeyType> KeyType { get; set; }
        public DbSet<KeyValue> KeyValue { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }

        public Context Clone()
        {
            return new Context(_options);
        }
        public void DetachAllEntities()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .ToList();

            foreach (var entry in changedEntries)
            {
                entry.State = EntityState.Detached;
            }
        }

    }
}
