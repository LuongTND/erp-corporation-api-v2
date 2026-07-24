using Microsoft.Extensions.Configuration;

namespace Contract;

public class AppConfiguration(IConfiguration configuration) : IAppConfiguration
{
    public JwtOptions GetJwtOptions()
    {
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

        if (jwtOptions == null)
            throw new InvalidOperationException("Missing 'Jwt' section in appsettings.json");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key) ||
            string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
            string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new InvalidOperationException("Jwt configuration is invalid. Key, Issuer, and Audience are required.");
        }

        return jwtOptions;
    }

    public EncryptionOptions GetEncryptionOptions()
    {
        var opts = configuration.GetSection("Encryption").Get<EncryptionOptions>();

        if (opts == null || string.IsNullOrWhiteSpace(opts.Key))
            throw new InvalidOperationException("Missing 'Encryption:Key' in appsettings.json");

        return opts;
    }
}
