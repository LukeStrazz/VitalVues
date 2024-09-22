using VVData.Data.Models;
using VVData.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using Data.Data.Models;

namespace VVData.Data;

public class DatabaseContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Person> People { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Fast> Fasts { get; set; }
    public DbSet<Bloodwork> Bloodworks { get; set; }
    public DbSet<BloodTest> BloodTests { get; set; }
    public DbSet<Test> Tests { get; set; }

    public override int SaveChanges()
    {
        var username = "N/A";

        this.ChangeTracker.DetectChanges();
        var added = this.ChangeTracker.Entries()
                    .Where(t => t.State == EntityState.Added)
                    .Select(t => t.Entity)
                    .ToArray();

        foreach (var entity in added)
        {
            if (entity is ITrack)
            {
                var track = entity as ITrack;
                track.CreatedDate = DateTime.Now;
                track.CreatedBy = username;
            }
        }

        var modified = this.ChangeTracker.Entries()
                    .Where(t => t.State == EntityState.Modified)
                    .Select(t => t.Entity)
                    .ToArray();

        foreach (var entity in modified)
        {
            if (entity is ITrack)
            {
                var track = entity as ITrack;
                track.ModifiedDate = DateTime.Now;
                track.ModifiedBy = username;
            }
        }
        return base.SaveChanges();
    }
}

