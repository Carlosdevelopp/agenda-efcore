using DataAccess.Contract;
using DataAccess.Models.Tables;
using Infrastructure.Contract;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Implementation;

public class MiAgendaInfrastructure : IMiAgendaInfrastructure
{
    private readonly IMiAgendaDataAccess _miAgendaDataAccess;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly ILogger<MiAgendaInfrastructure> _logger;
    private readonly IRedSocialHelper _redSocialHelper;
    private readonly ILocalFileStorageService _localFileStorage;
    
    public MiAgendaInfrastructure(IMiAgendaDataAccess miAgendaDataAccess,IPasswordHasher passwordHasher,IEmailService emailService,ILogger<MiAgendaInfrastructure> logger, 
                                  IRedSocialHelper redSocialHelper, ILocalFileStorageService localFileStorage)
    {
        _miAgendaDataAccess = miAgendaDataAccess;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _logger = logger;
        _redSocialHelper = redSocialHelper;
        _localFileStorage = localFileStorage;
    }

    #region Usuario
    public async Task<Usuario?> LoginAsync(string credencial, string password)
    {
        var usuario = await _miAgendaDataAccess.GetUserByCredentialAsync(credencial);

        if (usuario == null) return null;

        // verificar contraseña
        return _passwordHasher.Verify(usuario.Password, password) ? usuario : null;
    }
    public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDTO model)
    {
        string? rutaFoto = null;

        try
        {
            if (model.FotoUsuario != null)
            {
                rutaFoto = await _localFileStorage.SaveFileAsync(model.FotoUsuario, "usuarios");
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                Correo = model.Correo,
                NombreUsuario = model.NombreUsuario,
                Password = _passwordHasher.Hash(model.Password),
                Telefono = model.Telefono,
                RutaFoto = rutaFoto,
                FechaRegistro = DateTime.UtcNow,
                FechaAceptacionTerminos = DateTime.UtcNow,
                Estado = true
            };

            await _miAgendaDataAccess.RegisterUserAsync(usuario);

            return (true, "Usuario registrado correctamente.");
        }
        catch (DbUpdateException ex)
        {
            if (!string.IsNullOrWhiteSpace(rutaFoto))
                await _localFileStorage.DeleteFileAsync(rutaFoto);

            if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                return (false, "El correo o nombre de usuario ya está registrado.");

            throw;
        }
    }

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
    #endregion

    #region Contactos
    public async Task<List<Contacto>> GetContactsByUserIdAsync(int usuarioId)
    {
        return await _miAgendaDataAccess.GetContactsByUserIdAsync(usuarioId);
    }

    public async Task<Contacto?> GetContactByIdAsync(int contactoId)
    {
        return await _miAgendaDataAccess.GetContactByIdAsync(contactoId);
    }

    public async Task<Contacto> CreateContactAsync(CrearContactoDto dto, int usuarioId)
    {
        // Crear entidad
        var nuevoContacto = new Contacto
        {
            Nombre = dto.Nombre,
            PrimerApellido = dto.PrimerApellido,
            SegundoApellido = dto.SegundoApellido,
            FechaNacimiento = dto.FechaNacimiento,
            Telefono = dto.Telefono,
            FotoRuta = dto.FotoRuta, 
            UsuarioId = usuarioId,
            FechaRegistro = DateTime.Now,
            Detalle = new List<DetalleContactoRed>()
        };

        AgregarRedesSociales(nuevoContacto, dto.Instagram, dto.Facebook, dto.Twitter);

        if (nuevoContacto.Detalle.Any())
        {
            foreach (var d in nuevoContacto.Detalle)
            {
                Console.WriteLine($"  Red: TipoContactoId={d.TipoContactoId}, URL={d.URL}, ContactoId={d.ContactoId}");
            }
        }
        else
        {
            Console.WriteLine("NO SE AGREGARON REDES SOCIALES");
        }

        var resultado = await _miAgendaDataAccess.CreateContactAsync(nuevoContacto);

        if (resultado == null)
        {
            throw new InvalidOperationException("No se pudo crear el contacto en la base de datos");
        }

        _logger.LogInformation("Contacto creado: {ContactoId}", resultado.ContactoId);
        return resultado; 
    }

    private void AgregarRedesSociales(Contacto contacto, string instagram, string facebook, string twitter)
    {
        if (!string.IsNullOrWhiteSpace(instagram))
        {
            var url = _redSocialHelper.NormalizarUrlRedSocial(instagram, "instagram");
            if (url != null)
            {
                contacto.Detalle.Add(new DetalleContactoRed
                {
                    TipoContactoId = 1,
                    URL = url,
                    NombreUsuarioRed = _redSocialHelper.ExtraerUserName(url)!,
                    FechaRegistro = DateTime.Now
                });
            }
        }

        if (!string.IsNullOrWhiteSpace(facebook))
        {
            var url = _redSocialHelper.NormalizarUrlRedSocial(facebook, "facebook");
            if (url != null)
            {
                contacto.Detalle.Add(new DetalleContactoRed
                {
                    TipoContactoId = 2,
                    URL = url,
                    NombreUsuarioRed =  _redSocialHelper.ExtraerUserName(url)!, 
                    FechaRegistro = DateTime.Now
                });
            }
        }

        if (!string.IsNullOrWhiteSpace(twitter))
        {
            var url = _redSocialHelper.NormalizarUrlRedSocial(twitter, "twitter");
            if (url != null)
            {
                contacto.Detalle.Add(new DetalleContactoRed
                {
                    TipoContactoId = 3,
                    URL = url,
                    NombreUsuarioRed = _redSocialHelper.ExtraerUserName(url)!,
                    FechaRegistro = DateTime.Now
                });
            }
        }
    }

    public async Task<Contacto> UpdateContactAsync(ActualizarContactoDto dto, int usuarioId)
    {
        var contactoExistente = await _miAgendaDataAccess.GetContactByIdAsync(dto.ContactoId);

        if (contactoExistente == null)
            throw new KeyNotFoundException($"El contacto con el ID: {dto.ContactoId} no existe");

        if (contactoExistente.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("No tienes permiso para editar este contacto");

        contactoExistente.Nombre = dto.Nombre;
        contactoExistente.PrimerApellido = dto.PrimerApellido;
        contactoExistente.SegundoApellido = dto.SegundoApellido;
        contactoExistente.Telefono = dto.Telefono;
        contactoExistente.FechaNacimiento = dto.FechaNacimiento;

        if (!string.IsNullOrEmpty(dto.FotoRuta))
        {
            contactoExistente.FotoRuta = dto.FotoRuta;
        }

        //Actualizar redes sociales
        contactoExistente.Detalle.Clear();
        AgregarRedesSociales(contactoExistente, dto.Instagram, dto.Facebook, dto.Twitter);

        var result = await _miAgendaDataAccess.UpdateContactAsync(contactoExistente);

        if (!result)
            throw new InvalidOperationException("No se puede actualizar el contacto");

        _logger.LogInformation("Contacto {ContactoId} actualizado por usuario {UsuarioId}", dto.ContactoId, usuarioId);
        return contactoExistente;
    }

    public async Task<bool> DeleteContactAsync(int contactoId, int usuarioId)
    {
        var esDueño = await _miAgendaDataAccess.IsContactOwnerAsync(contactoId, usuarioId);

        if (!esDueño) return false;

        var contacto = await _miAgendaDataAccess.GetContactByIdAsync(contactoId);

        if (contacto?.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("No tienes permiso para eliminar este contacto"); 

        var result = await _miAgendaDataAccess.DeleteContactAsync(contactoId);

        if (result)
        {
            _logger.LogInformation("Contacto {ContactoId} eliminado por usuario {UsuarioId}", contactoId, usuarioId);
        }

        return result;
    }
    #endregion
}
