var builder = WebApplication.CreateBuilder(args);
EnvLoader.Load(builder);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.SeedIfEmptyAsync(db);
    await DbInitializer.SeedNotificationsIfMissingAsync(db);
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.UseCors("DevFrontend");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>(NotificationHub.HubPath);
app.MapHub<ChatHub>(ChatHub.HubPath);

app.Run();
