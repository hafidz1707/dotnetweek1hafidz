using WeekOneApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WeekOneApi.Infrastructure;

public class Depedencies
{
    public static void ConfigureService(IConfiguration configuration, IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={System.IO.Path.Join("./", "users.db")}"));
    }
}