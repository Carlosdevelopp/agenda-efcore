using DataAccess.Implementation.Base;
using DataAccess.Models.Tables;

namespace DataAccess.Contract;

public interface IMiAgendaDataAccess
{
    Task<Usuario?> GetUserByCredentialAsync(string credencial);

    Task<bool> ExistsUserAsync(string correo, string nombreeUsuario);

    Task<Usuario> RegisterAsync(Usuario usuario);

    Task<Usuario?> GetUserByIdAsync(int usuarioId);

    Task UpdatePasswordAsync(int usuarioId, string passwordHash);

    Task CreatePasswordResetTokenAsync(PasswordResetToken token);

    Task<PasswordResetToken?> GetPasswordResetTokenAsync(string tokenHash);

    Task MarkPasswordResetTokenUsedAsync(int tokenId);

    Task InvalidatePasswordResetTokensAsync(int usuarioId);

    Task<List<Contacto>> GetContactsByUserIdAsync(int usuarioId);

    Task<Contacto?> GetContactByIdAsync(int contactoId);

    Task<Contacto> CreateContactAsync(Contacto contacto);

    Task<bool> UpdateUserAsync(Usuario model);

    Task<bool> UpdateContactAsync(Contacto contacto);

    Task<bool> DeleteContactAsync(int contactoId);

    Task<bool> ContactExistsAsync(int contactoId);

    Task<bool> IsContactOwnerAsync(int contactoId, int usuarioId);
}
