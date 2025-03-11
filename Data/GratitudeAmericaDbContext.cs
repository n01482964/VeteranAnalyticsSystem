using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Data
{
    public class GratitudeAmericaDbContext : DbContext
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
            base.OnModelCreating(modelBuilder);

            // Explicitly ignore the 'Responses' property in Survey
            modelBuilder.Entity<Survey>().Ignore(s => s.Responses);

        }
    }
}
