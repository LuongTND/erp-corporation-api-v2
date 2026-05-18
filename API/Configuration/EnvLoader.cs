namespace API.Configuration;

public static class EnvLoader
{
    private static readonly (string EnvKey, string ConfigKey)[] Mappings =
    [
        // Database
        ("CONNECTION_STRING", "ConnectionStrings:DefaultConnection"),
        ("SQL_SERVER_FOR_DAPPER_CONNECTION", "ConnectionStrings:Dapper"),

        // Redis
        ("REDIS_CONNECTION", "Redis:ConnectionString"),

        // Azure Blob
        ("AZURE_BLOB_STORAGE", "AzureBlob:ConnectionString"),

        // JWT
        ("JWT_KEY", "Jwt:SecretKey"),
        ("JWT_ISSUER", "Jwt:Issuer"),
        ("JWT_AUDIENCE", "Jwt:Audience"),
        ("JWT_ACCESS_TOKEN_EXPIRATION", "Jwt:AccessTokenExpirationMinutes"),
        ("JWT_REFRESH_TOKEN_EXPIRATION", "Jwt:RefreshTokenExpirationDays"),

        // Mail (SMTP)
        ("MAIL_SETTINGS_MAIL", "MailSettings:Mail"),
        ("MAIL_SETTINGS_DISPLAY_NAME", "MailSettings:DisplayName"),
        ("MAIL_SETTINGS_PASSWORD", "MailSettings:Password"),
        ("MAIL_SETTINGS_HOST", "MailSettings:Host"),
        ("MAIL_SETTINGS_PORT", "MailSettings:Port"),

        // MISA (hóa đơn điện tử)
        ("MISA_SETTINGS_APP_ID", "MisaSettings:AppId"),
        ("MISA_SETTINGS_TAX_CODE", "MisaSettings:TaxCode"),
        ("MISA_SETTINGS_USERNAME", "MisaSettings:Username"),
        ("MISA_SETTINGS_PASSWORD", "MisaSettings:Password"),
        ("MISA_SETTINGS_INV_SERIES", "MisaSettings:InvSeries"),
        ("MISA_SETTINGS_VAT_RATE", "MisaSettings:VatRate"),
        ("MISA_SETTINGS_BASE_URL", "MisaSettings:BaseUrl"),
        ("MISA_SETTINGS_AUTH_ENDPOINT", "MisaSettings:AuthEndpoint"),
        ("MISA_SETTINGS_INVOICE_ENDPOINT", "MisaSettings:InvoiceEndpoint"),

        // Zalo
        ("ZALO_APP_SECRET", "ZaloSettings:AppSecret"),
        ("ZALO_APP_ID", "ZaloSettings:AppId"),

        // MPOS
        ("MPOS_MERCHANT_ID", "MposSettings:MerchantId"),
        ("MPOS_SECRET_KEY", "MposSettings:SecretKey"),
        ("MPOS_MUID", "MposSettings:Muid"),
        ("MPOS_PASS_POS", "MposSettings:PassPos"),
        ("MPOS_POSID", "MposSettings:PosId"),
        ("MPOS_BASE_URL", "MposSettings:BaseUrl"),
    ];

    public static void Load(WebApplicationBuilder builder)
    {
        var envFile = ResolveEnvFilePath(builder.Environment.ContentRootPath);
        if (envFile is not null)
            DotNetEnv.Env.Load(envFile);

        foreach (var (envKey, configKey) in Mappings)
            MapIfPresent(builder.Configuration, envKey, configKey);
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
