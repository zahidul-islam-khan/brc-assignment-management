using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BRC.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

        // Convert URI-style connection strings (postgresql://user:pass@host/db)
        // to ADO.NET format (Host=...;Database=...;Username=...;Password=...)
        if (connectionString.StartsWith("postgresql://") || connectionString.StartsWith("postgres://"))
        {
            connectionString = ConvertPostgresUri(connectionString);
        }

        services.AddDbContext<BrcDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<BRC.Infrastructure.Data.Seed.DataSeeder>();

        return services;
    }

    private static string ConvertPostgresUri(string uri)
    {
        var uriObj = new Uri(uri);
        var userInfo = uriObj.UserInfo.Split(':');
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var host = uriObj.Host;
        var port = uriObj.Port > 0 ? uriObj.Port : 5432;
        var database = uriObj.AbsolutePath.TrimStart('/');

        var query = System.Web.HttpUtility.ParseQueryString(uriObj.Query);
        var sslMode = query["sslmode"] ?? "Require";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true";
    }
}
