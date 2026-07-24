var builder = WebApplication.CreateBuilder(args);

EnvLoader.Load(builder);

builder.Host.AddSerilogLogging();

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrationsAndSeedAsync();

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
