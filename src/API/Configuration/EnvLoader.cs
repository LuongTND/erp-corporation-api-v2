namespace API;

public static class EnvLoader
{
    private static readonly IReadOnlyDictionary<string, string> ExplicitKeyMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["CONNECTION_STRING"] = "ConnectionStrings:SqlServerConnection",
            ["REDIS_CONNECTION"]  = "Upstash:ConnectionString",
            ["ENCRYPTION_KEY"]    = "Encryption:Key",
        };

    private static readonly (string Prefix, string Section, IReadOnlyDictionary<string, string> Aliases)[] PrefixMaps =
    [
        ("JWT_", "Jwt", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["KEY"]                      = "Key",
            ["ISSUER"]                   = "Issuer",
            ["AUDIENCE"]                 = "Audience",
            ["ACCESS_TOKEN_EXPIRATION"]  = "AccessTokenLifetime",
            ["REFRESH_TOKEN_EXPIRATION"] = "RefreshTokenLifetime",
        }),
        ("MAIL_SETTINGS_", "EmailSettings", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MAIL"]         = "SenderEmail",
            ["DISPLAY_NAME"] = "SenderName",
            ["PASSWORD"]     = "AppPassword",
            ["HOST"]         = "SmtpHost",
            ["PORT"]         = "SmtpPort",
        }),
    ];

    public static void Load(WebApplicationBuilder builder)
    {
        var envFile = ResolveEnvFilePath(builder.Environment.ContentRootPath);
        if (envFile is not null)
            DotNetEnv.Env.Load(envFile);

        MapToConfiguration(builder.Configuration);
    }

    private static string? ResolveEnvFilePath(string contentRoot)
    {
        string[] candidates =
        [
            Path.Combine(contentRoot, ".env"),
            Path.Combine(contentRoot, "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(AppContext.BaseDirectory, ".env"),
        ];

        return candidates.FirstOrDefault(File.Exists);
    }

    private static void MapToConfiguration(ConfigurationManager configuration)
    {
        var allKeys = ExplicitKeyMap.Keys
            .Concat(PrefixMaps.SelectMany(p => GetKnownKeysForPrefix(p.Prefix, p.Aliases)));

        foreach (var envKey in allKeys)
        {
            var configKey = ResolveConfigKey(envKey);
            if (configKey is null) continue;
            MapIfPresent(configuration, envKey, configKey);
        }
    }

    private static IEnumerable<string> GetKnownKeysForPrefix(
        string prefix,
        IReadOnlyDictionary<string, string> aliases)
        => aliases.Keys.Select(suffix => prefix + suffix);

    private static string? ResolveConfigKey(string envKey)
    {
        if (ExplicitKeyMap.TryGetValue(envKey, out var explicitKey))
            return explicitKey;

        foreach (var (prefix, section, aliases) in PrefixMaps)
        {
            if (!envKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            var suffix = envKey[prefix.Length..];
            if (string.IsNullOrWhiteSpace(suffix)) continue;

            if (!aliases.TryGetValue(suffix, out var property))
                property = ToPascalCase(suffix);

            return $"{section}:{property}";
        }

        return null;
    }

    private static void MapIfPresent(ConfigurationManager configuration, string envKey, string configKey)
    {
        var raw = Environment.GetEnvironmentVariable(envKey) ?? configuration[envKey];
        if (string.IsNullOrWhiteSpace(raw)) return;
        configuration[configKey] = TrimEnvQuotes(raw.Trim());
    }

    private static string ToPascalCase(string value)
    {
        var parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return string.Concat(parts.Select(p =>
            p.Length == 0 ? "" : char.ToUpperInvariant(p[0]) + p[1..].ToLowerInvariant()));
    }

    private static string TrimEnvQuotes(string value) =>
        value.Length >= 2 && value[0] == '"' && value[^1] == '"'
            ? value[1..^1]
            : value;
}
