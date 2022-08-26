using Microsoft.EntityFrameworkCore;
using WeekOneApi.Infrastructure.Data.Models;
using WeekOneApi.Infrastructure.Shared;
using System.Reflection;

namespace WeekOneApi.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public DbSet<User> Users {get; set;}
    public DbSet<AuthToken> AuthTokens {get; set;}
    public DbSet<UserChanger> UsersChanger {get; set;}
    public DbSet<ServiceList> ServiceLists {get; set;}
    public DbSet<CircleCheck> CircleChecks {get; set;}
    public DbSet<ExteriorView> ExteriorViews {get; set;}
    public DbSet<TireView> TireViews {get; set;}
    public DbSet<InteriorView> InteriorViews {get; set;}
    public DbSet<ComplaintView> ComplaintViews {get; set;}
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
