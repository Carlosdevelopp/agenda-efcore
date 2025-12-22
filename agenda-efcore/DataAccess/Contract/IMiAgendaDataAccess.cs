using DataAccess.Models.Tables;

namespace DataAccess.Contract;

public interface IMiAgendaDataAccess
{
    #region GET
    Task<Usuario?> GetUserByCredentialAsync(string credencial);

    Task<bool> ExistsAsync(string correo, string nombreeUsuario);

    Task<Usuario> RegisterAsync(Usuario usuario);

    Task<Usuario?> GetUserByIdAsync(int usuarioId);

    Task UpdatePasswordAsync(int usuarioId, string passwordHash);

    Task<List<Contacto>> GetContactById(int usuarioId);

    Task CreatePasswordResetTokenAsync(PasswordResetToken token);

    Task<PasswordResetToken?> GetPasswordResetTokenAsync(string tokenHash);

    Task MarkPasswordResetTokenAsUsedAsync(int tokenId);
    #endregion
}
