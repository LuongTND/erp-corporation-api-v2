using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("Applying migrations...");
        await db.Database.MigrateAsync();

        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine(">>> [[[Seeding development data]]]");
            await AppData.SeedAsync(db);
            Console.WriteLine(">>> >>> Seeding completed!");
        }

        await AppData.SyncPermissionsAsync(db);
        Console.WriteLine("Permissions synced.");

        // await AppData.SyncRoleHierarchyAsync(db);
        Console.WriteLine("Role hierarchy synced.");
    }
}
