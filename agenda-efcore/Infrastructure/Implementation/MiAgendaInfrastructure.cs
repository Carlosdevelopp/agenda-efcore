using DataAccess.Contract;
using DataAccess.Models.Tables;
using Infrastructure.Contract;

namespace Infrastructure.Implementation;

public class MiAgendaInfrastructure : IMiAgendaInfrastructure
{
    private readonly IMiAgendaDataAccess _miAgendaDataAccess;
    private readonly IPasswordHasher _passwordHasher;

    public MiAgendaInfrastructure(IMiAgendaDataAccess miAgendaDataAccess, IPasswordHasher passwordHasher)
    {
        _miAgendaDataAccess = miAgendaDataAccess;
        _passwordHasher = passwordHasher;
    }

    #region GET
    public async Task<Usuario?> LoginAsync(string credencial, string password)
    {
        var usuario = await _miAgendaDataAccess.GetUserByCredentialAsync(credencial);

        if (usuario == null) return null;

        // verificar contraseña
        return _passwordHasher.Verify(usuario.Password, password) ? usuario : null;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(Usuario model)
    {
        bool existe = await _miAgendaDataAccess.ExistsAsync(model.Correo, model.NombreUsuario);

        if (existe)
            return (false,"Ël correo o nombre de usuario ya está registrado.");

        model.Password = _passwordHasher.Hash(model.Password);

        model.FechaRegistro = DateTime.Now;
        model.Estado = true;

        await _miAgendaDataAccess.RegisterAsync(model);

        return (true, "Usuario registrado correctamente.");
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(int usuarioId, string newPassword)
    {

        var passwordHash = _passwordHasher.Hash(newPassword);

        await _miAgendaDataAccess.UpdatePasswordAsync(usuarioId, newPassword);

        return (true, "La contraseña fue actualizada correctamente.");
    }

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
