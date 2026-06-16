namespace API.Configuration;

public static class EnvLoader
{
    private static readonly IReadOnlyDictionary<string, string> ExplicitKeyMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Database
            ["CONNECTION_STRING"] = "ConnectionStrings:DefaultConnection",
            ["SQL_SERVER_FOR_DAPPER_CONNECTION"] = "ConnectionStrings:Dapper",

            // Redis
            ["REDIS_CONNECTION"] = "Redis:ConnectionString",

            // Azure Blob
            ["AZURE_BLOB_STORAGE"] = "AzureBlob:ConnectionString",
        };

    private static readonly IReadOnlyDictionary<string, string> JwtKeyAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["KEY"] = "SecretKey",
            ["ISSUER"] = "Issuer",
            ["AUDIENCE"] = "Audience",
            ["ACCESS_TOKEN_EXPIRATION"] = "AccessTokenExpirationMinutes",
            ["REFRESH_TOKEN_EXPIRATION"] = "RefreshTokenExpirationDays",
        };

    private static readonly IReadOnlyDictionary<string, string> MailKeyAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MAIL"] = "Mail",
            ["DISPLAY_NAME"] = "DisplayName",
            ["PASSWORD"] = "Password",
            ["HOST"] = "Host",
            ["PORT"] = "Port",
        };

    private static readonly IReadOnlyDictionary<string, string> MisaKeyAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["APP_ID"] = "AppId",
            ["TAX_CODE"] = "TaxCode",
            ["USERNAME"] = "Username",
            ["PASSWORD"] = "Password",
            ["INV_SERIES"] = "InvSeries",
            ["VAT_RATE"] = "VatRate",
            ["BASE_URL"] = "BaseUrl",
            ["AUTH_ENDPOINT"] = "AuthEndpoint",
            ["INVOICE_ENDPOINT"] = "InvoiceEndpoint",
        };

    private static readonly IReadOnlyDictionary<string, string> ZaloKeyAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["APP_SECRET"] = "AppSecret",
            ["APP_ID"] = "AppId",
        };

    private static readonly IReadOnlyDictionary<string, string> MposKeyAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MERCHANT_ID"] = "MerchantId",
            ["SECRET_KEY"] = "SecretKey",
            ["MUID"] = "Muid",
            ["PASS_POS"] = "PassPos",
            ["POSID"] = "PosId",
            ["BASE_URL"] = "BaseUrl",
        };

    public static void Load(WebApplicationBuilder builder)
    {
        var envFile = ResolveEnvFilePath(builder.Environment.ContentRootPath);
        if (envFile is not null)
            DotNetEnv.Env.Load(envFile);

        MapByConvention(builder.Configuration);
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

    private static void MapByConvention(Microsoft.Extensions.Configuration.ConfigurationManager configuration)
    {
        var envVars = Environment.GetEnvironmentVariables();
        foreach (var keyObj in envVars.Keys)
        {
            if (keyObj is not string envKey || string.IsNullOrWhiteSpace(envKey))
                continue;

            var configKey = ResolveConfigKey(envKey);
            if (configKey is null)
                continue;

            MapIfPresent(configuration, envKey, configKey);
        }
    }

    private static string? ResolveConfigKey(string envKey)
    {
        if (ExplicitKeyMap.TryGetValue(envKey, out var explicitKey))
            return explicitKey;

        if (TryResolvePrefixedKey(envKey, "JWT_", "Jwt", JwtKeyAliases, out var jwtKey))
            return jwtKey;

        if (TryResolvePrefixedKey(envKey, "MAIL_SETTINGS_", "MailSettings", MailKeyAliases, out var mailKey))
            return mailKey;

        if (TryResolvePrefixedKey(envKey, "MISA_SETTINGS_", "MisaSettings", MisaKeyAliases, out var misaKey))
            return misaKey;

        if (TryResolvePrefixedKey(envKey, "ZALO_", "ZaloSettings", ZaloKeyAliases, out var zaloKey))
            return zaloKey;

        if (TryResolvePrefixedKey(envKey, "MPOS_", "MposSettings", MposKeyAliases, out var mposKey))
            return mposKey;

        return null;
    }

    private static bool TryResolvePrefixedKey(
        string envKey,
        string prefix,
        string section,
        IReadOnlyDictionary<string, string> aliases,
        out string? configKey)
    {
        configKey = null;
        if (!envKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        var suffix = envKey[prefix.Length..];
        if (string.IsNullOrWhiteSpace(suffix))
            return false;

        if (!aliases.TryGetValue(suffix, out var property))
            property = ToPascalCase(suffix);

        configKey = $"{section}:{property}";
        return true;
    }

    private static string ToPascalCase(string value)
    {
        // VALUE_LIKE_THIS -> ValueLikeThis
        var parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return value;

        return string.Concat(parts.Select(p => p.Length == 0 ? "" : char.ToUpperInvariant(p[0]) + p[1..].ToLowerInvariant()));
    }

    private static void MapIfPresent(
        Microsoft.Extensions.Configuration.ConfigurationManager configuration,
        string envKey,
        string configKey)
    {
        var raw =
            Environment.GetEnvironmentVariable(envKey)
            ?? configuration[envKey];

        if (string.IsNullOrWhiteSpace(raw))
            return;

        configuration[configKey] = TrimEnvQuotes(raw.Trim());
    }

    /// <summary>
    /// Bỏ ngoặc kép bọc giá trị trong .env (ví dụ mật khẩu Gmail).
    /// </summary>
    private static string TrimEnvQuotes(string value)
    {
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            return value[1..^1];
        return value;
    }
}
