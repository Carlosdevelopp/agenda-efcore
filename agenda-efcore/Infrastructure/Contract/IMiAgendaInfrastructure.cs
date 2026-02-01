using DataAccess.Models.Tables;
using Infrastructure.DTOs;

namespace Infrastructure.Contract;

public interface IMiAgendaInfrastructure
{
    #region GET
    Task<Usuario?> LoginAsync(string credencial, string password);
    Task<List<Contacto>> GetContactsByUserIdAsync(int usuarioId);
    #endregion

    #region POST
    Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDTO model);
    #endregion

    #region UPDATE
    Task<(bool Success, string Message)> ResetPasswordAsync(string token, string newPassword);
    #endregion

    Task<(bool Success, string Message)> ForgotPasswordAsync(string email);

    int CalcularEdad(DateTime FechaNacimiento);

    Task<Contacto?> GetContactByIdAsync(int contactoId);

    Task<Contacto> CreateContactAsync(CrearContactoDto dto, int usuarioId);

    Task<Contacto> UpdateContactAsync(ActualizarContactoDto dto, int usuarioId);

     Task<bool> DeleteContactAsync(int contactoId, int usuarioId);
}
