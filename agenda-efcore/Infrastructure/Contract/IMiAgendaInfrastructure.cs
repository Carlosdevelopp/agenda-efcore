using DataAccess.Models.Tables;

namespace Infrastructure.Contract;

public interface IMiAgendaInfrastructure
{
    #region GET
    Task<Usuario?> LoginAsync(string credencial, string password);
    Task<List<Contacto>> GetContactByIdAsync(int usuarioId);
    #endregion

    #region POST
    Task<(bool Success, string Message)> RegisterAsync(Usuario model);
    #endregion

    #region UPDATE
    Task<(bool Success, string Message)> ResetPasswordAsync(string token, string newPassword);
    #endregion

    Task<(bool Success, string Message)> ForgotPasswordAsync(string email);

    int CalcularEdad(DateTime FechaNacimiento);
}
