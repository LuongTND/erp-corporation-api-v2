var builder = WebApplication.CreateBuilder(args);

EnvLoader.Load(builder);

builder.Host.AddSerilogLogging();

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrationsAndSeedAsync(typeof(Program).Assembly);

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHubs();
app.MapHealthChecks("/health");

app.Run();
