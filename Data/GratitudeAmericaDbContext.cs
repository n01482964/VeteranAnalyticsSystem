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

        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Veteran> Veterans { get; set; }
    }
}