using DataAccess.Models.Tables;

namespace DataAccess.Contract;

public interface IMiAgendaDataAccess
{
    #region GET
    Task<Usuario?> GetUserByCredentialAsync(string credencial);

    Task<bool> ExistsAsync(string correo, string nombreeUsuario);

    Task<Usuario> CreateUserAsync(Usuario usuario);

    //Task AddUserAsync(Usuario usuario);

    Task<List<Contacto>> GetContactById(int usuarioId);

    Task<List<Usuario>> GetAllUsersAsync();
    #endregion
}
