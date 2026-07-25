namespace Application;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hashed);
}
