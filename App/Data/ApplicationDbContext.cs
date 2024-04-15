using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using App.Models; // This assumes your models are in the App.Models namespace.

namespace App.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Use your ApplicationUser class here
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EnvironmentalIncident> EnvironmentalIncidents { get; set; } // Add your EnvironmentalIncident DbSet

        // Override OnModelCreating if you need to add any custom model configuration.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // If you have any custom model configuration, do it here.
            // For example, you might need to configure a compound key, set up relationships, or define indices.
        }
    }
}
