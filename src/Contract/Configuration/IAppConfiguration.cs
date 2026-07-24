namespace Contract;

public interface IAppConfiguration
{
    JwtOptions GetJwtOptions();
    EncryptionOptions GetEncryptionOptions();
}
