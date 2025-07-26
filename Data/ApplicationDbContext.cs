using GymLinkPro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymLinkPro.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<ProjectLink> ProjectLinks { get; set; }

        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<ClassRegistration> ClassRegistrations { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<GymClass> GymClasses { get; set; }
    }
}
