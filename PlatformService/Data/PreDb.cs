using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PreDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
        }
    }

    private static void SeedData(AppDbContext? appDbContext, bool isProduction)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);

        if (isProduction)
        {
            try
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                appDbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }
        }

        if (!appDbContext.Platforms.Any())
        {
            Console.WriteLine("Seeding data...");
            appDbContext.Platforms.AddRange(
                new Platform { Name = "Dotnet", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );
            appDbContext.SaveChanges();
        }
        else
        {
            Console.WriteLine("Data already exists");
        }
    }
}