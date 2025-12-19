using System;
using System.Collections.Generic;
using System.Linq;
namespace Infrastructure.Contract;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
