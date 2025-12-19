using Infrastructure.Contract;
using Isopoh.Cryptography.Argon2;

namespace Infrastructure.Implementation.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return Argon2.Hash(password);
    }

    public bool Verify(string hash, string password)
    {
        return Argon2.Verify(hash, password);
    }
}
