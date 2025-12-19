using DataAccess.Contract;
using DataAccess.Models.Tables;
using Infrastructure.Contract;
using Isopoh.Cryptography.Argon2;

namespace Infrastructure.Implementation;

public class MiAgendaInfrastructure : IMiAgendaInfrastructure
{
    private readonly IMiAgendaDataAccess _miAgendaDataAccess;

    public MiAgendaInfrastructure(IMiAgendaDataAccess miAgendaDataAccess)
    {
        _miAgendaDataAccess = miAgendaDataAccess;
    }

    #region GET
    public async Task<Usuario?> LoginAsync(string credencial, string password)
    {
        var usuario = await _miAgendaDataAccess.GetUserByCredentialAsync(credencial);
        bool credencialCoincide = (usuario != null);
        if (usuario == null) return null;

        // Verificar contraseña
        //bool passwordHash = Argon2.Verify(password);

        if (usuario.Password == password)
        {
            return usuario;
        }
        else
        {
            return null;
        }

        //Console.WriteLine($"Usuario encontrado: {usuario?.NombreUsuario ?? "Ninguno"}");
        //Console.WriteLine($"Password BD: '{usuario?.Password}' - Password Input: '{password}'");

        
        //return passwordHash ? usuario : null;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(Usuario model)
    {
        bool existe = await _miAgendaDataAccess.ExistsAsync(model.Correo, model.NombreUsuario);

        if (existe)
            return (false,"Ël correo o nombre de usuario ya está registrado.");
        
        var passwordHash = Argon2.Hash(model.Password);

        var Nuevousuario = new Usuario
        {
            Nombre = model.Nombre,
            PrimerApellido = model.PrimerApellido,
            SegundoApellido = model.SegundoApellido,
            Telefono = model.Telefono,
            Correo = model.Correo,
            NombreUsuario = model.NombreUsuario,
            Password = passwordHash
        };

        await _miAgendaDataAccess.CreateUserAsync(Nuevousuario);

        return (true, "Usuario registrado correctamente.");
    }


    //private bool VerifyPasswordArgon2(string password, string hashedPassword)
    //{
    //    var fullBytes = Convert.FromBase64String(hashedPassword);
    //    byte[] salt = fullBytes.Take(16).ToArray();
    //    byte[] hash = fullBytes.Skip(16).ToArray();

    //    var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
    //    {
    //        Salt = salt,
    //        DegreeOfParallelism = 8,
    //        Iterations = 4,
    //        MemorySize = 1024 * 64
    //    };

    //    byte[] testHash = argon2.GetBytes(32);
    //    return testHash.SequenceEqual(hash);
    //}

    public async Task<List<Contacto>> GetContactByIdAsync(int usuarioId)
    {
        return await _miAgendaDataAccess.GetContactById(usuarioId);
    }

    public int CalcularEdad(DateTime FechaNacimiento)
    {
        var hoy = DateTime.Now;
        int edad = hoy.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        return edad;
    }
    #endregion
}
