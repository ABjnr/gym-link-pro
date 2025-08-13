using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymLinkPro.Models;

namespace GymLinkPro.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<ProjectLink> ProjectLinks { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ClassRegistration> ClassRegistrations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<GymClass> GymClasses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure cascade delete for ClassRegistration dependencies
            modelBuilder.Entity<ClassRegistration>()
                .HasOne<User>()
                .WithMany() // If you later add a navigation property, use .WithMany(u => u.ClassRegistrations)
                .HasForeignKey(cr => cr.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassRegistration>()
                .HasOne<GymClass>()
                .WithMany() // Adjust if you add a navigation property
                .HasForeignKey(cr => cr.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure cascade delete for projects – deleting a project also removes its links and members
            modelBuilder.Entity<Project>()
                .HasMany<ProjectLink>()
                .WithOne() // Again adjust if you add navigation properties
                .HasForeignKey(pl => pl.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany<ProjectMember>()
                .WithOne() // Adjust for navigation properties when added
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
