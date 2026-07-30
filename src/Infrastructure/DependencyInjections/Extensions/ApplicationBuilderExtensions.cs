using Microsoft.AspNetCore.Builder;

namespace Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app, Assembly apiAssembly)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var log = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        log.LogInformation("Applying migrations...");
        await db.Database.MigrateAsync();
        log.LogInformation("Migrations applied");

        if (app.Environment.IsDevelopment())
        {
            log.LogInformation("Seeding development data...");
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await AppData.SeedAsync(db, hasher);
            log.LogInformation("Seeding completed");
        }

        log.LogInformation("Syncing permissions...");
        await AppData.SyncPermissionsAsync(db, apiAssembly);
        log.LogInformation("Permissions synced");
    }
}
