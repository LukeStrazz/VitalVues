using AiDietPlanData.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace AiDietPlanData.Data;

public class DatabaseContext : DbContext
{
	public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
    {
    }

    public DbSet<Person> People { get; set; }
    public DbSet<Goal> Goals { get; set; }
}

