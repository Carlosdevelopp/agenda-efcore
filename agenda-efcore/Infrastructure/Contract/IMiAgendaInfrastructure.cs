using DataAccess.Models.Tables;

namespace Infrastructure.Contract;

public interface IMiAgendaInfrastructure
{
    #region GET
    Task<Usuario?> LoginAsync(string credencial, string password);
    Task<(bool Success, string Message)> RegisterAsync(Usuario model);
    Task<List<Contacto>> GetContactByIdAsync(int usuarioId);
    int CalcularEdad(DateTime FechaNacimiento);
    Task<List<Usuario>> GetAllUsersAsync();
    #endregion
}
