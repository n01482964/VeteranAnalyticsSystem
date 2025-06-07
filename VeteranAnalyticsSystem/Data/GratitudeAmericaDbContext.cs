using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Data
{
    public class GratitudeAmericaDbContext : IdentityDbContext<ApplicationUser>
    {
        public GratitudeAmericaDbContext(DbContextOptions<GratitudeAmericaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Volunteer> Volunteers { get; set; } = default!;
        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Veteran> Veterans { get; set; } = default!;
        public DbSet<Survey> Surveys { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call the base method when using IdentityDbContext
            base.OnModelCreating(modelBuilder);

            // Custom model configuration (optional)
            modelBuilder.Entity<Survey>().Ignore(s => s.Responses);
        }
    }
}
