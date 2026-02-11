
using PlatformService.Models;

namespace PlatformService.Data;

public static class PreDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
        }
    }

    private static void SeedData(AppDbContext? appDbContext)
    {
        if (appDbContext == null)
        {
            throw new ArgumentNullException(nameof(appDbContext));
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