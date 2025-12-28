using DataAccess.Contract;
using DataAccess.Models.Tables;
using Infrastructure.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Implementation;

public class MiAgendaInfrastructure : IMiAgendaInfrastructure
{
    private readonly IMiAgendaDataAccess _miAgendaDataAccess;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public MiAgendaInfrastructure(IMiAgendaDataAccess miAgendaDataAccess, IPasswordHasher passwordHasher, IEmailService emailService,  IConfiguration configuration)
    {
        _miAgendaDataAccess = miAgendaDataAccess;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _configuration = configuration;
    }

    #region GET
    public async Task<Usuario?> LoginAsync(string credencial, string password)
    {
        var usuario = await _miAgendaDataAccess.GetUserByCredentialAsync(credencial);

        if (usuario == null) return null;

        // verificar contraseña
        return _passwordHasher.Verify(usuario.Password, password) ? usuario : null;
    }

    public async Task<List<Contacto>> GetContactByIdAsync(int usuarioId)
    {
        return await _miAgendaDataAccess.GetContactById(usuarioId);
    }
    #endregion

    #region POST
    public async Task<(bool Success, string Message)> RegisterAsync(Usuario model)
    {
        bool existe = await _miAgendaDataAccess.ExistsAsync(model.Correo, model.NombreUsuario);

        if (existe)
            return (false, "Ël correo o nombre de usuario ya está registrado.");

        model.Password = _passwordHasher.Hash(model.Password);

        model.FechaRegistro = DateTime.Now;
        model.Estado = true;

        await _miAgendaDataAccess.RegisterAsync(model);

        return (true, "Usuario registrado correctamente.");
    }
    #endregion

    #region UPDATE
    public async Task<(bool Success, string Message)> ResetPasswordAsync(string rawToken, string newPassword)
    {
        //Hashear token recibido
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        //Buscar token valido
        var resetToken = await _miAgendaDataAccess.GetPasswordResetTokenAsync(tokenHash);

        if (resetToken == null)
            return (false, "El enlace de recuperación es inválido o ha expirado.");

        //Hashear nueva contraseña con Argon2
        var passwordHash = _passwordHasher.Hash(newPassword);

        //Actualizar contraseña
        await _miAgendaDataAccess.UpdatePasswordAsync(resetToken.UsuarioId, passwordHash);

        //Marcar token como usado
        await _miAgendaDataAccess.MarkPasswordResetTokenUsedAsync(resetToken.PasswordResetTokenId);

        //Invalidar otros tokens
        await _miAgendaDataAccess.InvalidatePasswordResetTokensAsync(resetToken.UsuarioId);

        return (true, "La contraseña fue actualizada correctamente.");
    }
    #endregion

    public async Task<(bool Success, string Message)> ForgotPasswordAsync(string email)
    {
        var usuario = await _miAgendaDataAccess.GetUserByCredentialAsync(email);

        //No revelar si el correo existe
        if (usuario == null)
            return (true, "Si el correo existe, recibiás instrucciones para restablecer tu contraseña.");

        //Invalidar tokens anteriores
        await _miAgendaDataAccess.InvalidatePasswordResetTokensAsync(usuario.UsuarioId);

        //Genarar token seguro
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        //Hashear del token (No guardar plano)
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        //Crear token
        var resetToken = new PasswordResetToken
        {
            UsuarioId = usuario.UsuarioId,
            TokenHash = tokenHash,
            Expiration = DateTime.UtcNow.AddHours(1),
            Used = false 
        };

        await _miAgendaDataAccess.CreatePasswordResetTokenAsync(resetToken);

        //Envío de email
        await _emailService.SendPasswordResetAsync(
            usuario.Correo,
            usuario.NombreUsuario,
            rawToken
            );

        return (true, "Si el correo existe, recibirás instrucciones para restablecer tu contraseña.");
    }

    public int CalcularEdad(DateTime FechaNacimiento)
    {
        var hoy = DateTime.Now;
        int edad = hoy.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
        return edad;
    }
}
