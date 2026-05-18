namespace API.Configuration;

public static class EnvLoader
{
    public static void Load(WebApplicationBuilder builder)
    {
        var envFile = ResolveEnvFilePath(builder.Environment.ContentRootPath);
        if (envFile is not null)
            DotNetEnv.Env.Load(envFile);

        MapConnectionString(builder);
    }

    private static string? ResolveEnvFilePath(string contentRoot)
    {
        var candidates = new[]
        {
            Path.Combine(contentRoot, ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(AppContext.BaseDirectory, ".env")
        };

        return candidates.FirstOrDefault(File.Exists);
    }

    private static void MapConnectionString(WebApplicationBuilder builder)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? builder.Configuration["CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    }
}
