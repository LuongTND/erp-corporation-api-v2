namespace Contract;

public sealed class JwtOptions
{
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int AccessTokenLifetime { get; set; }
    public int RefreshTokenLifetime { get; set; }
}