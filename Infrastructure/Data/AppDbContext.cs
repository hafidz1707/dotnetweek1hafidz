using Microsoft.EntityFrameworkCore;
using WeekOneApi.Infrastructure.Data.Models;
using System.Reflection;

namespace WeekOneApi.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public DbSet<User> Users {get; set;}
    public DbSet<AuthToken> AuthTokens {get; set;}
    public DbSet<UserChanger> UsersChanger {get; set;}

    public AppDbContext(DbContextOptions options) : base(options)
    {
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    //     => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
