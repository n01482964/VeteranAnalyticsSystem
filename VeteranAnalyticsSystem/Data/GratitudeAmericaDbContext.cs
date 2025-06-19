using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Models.Core;

namespace VeteranAnalyticsSystem.Data;

public class GratitudeAmericaDbContext(DbContextOptions<GratitudeAmericaDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Volunteer> Volunteers { get; set; } = default!;

    public DbSet<Event> Events { get; set; } = default!;

    public DbSet<Veteran> Veterans { get; set; } = default!;

    public DbSet<Survey> Surveys { get; set; } = default!;

    public DbSet<SyncRecord> SyncRecords { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
