using DataAccess.Contract;
using DataAccess.Implementation.Base;
using DataAccess.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementation;

public class MiAgendaDataAccess : IMiAgendaDataAccess
{
    private readonly ApplicationDbContext _applicationDbContext;

    public MiAgendaDataAccess(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    #region Usuarios
    public async Task<Usuario?> GetUserByCredentialAsync(string credencial)
    {
        return await _applicationDbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == credencial || u.NombreUsuario == credencial);
    }

    public async Task<bool> ExistsUserAsync(string correo, string nombreUsuario)
    {
        return await _applicationDbContext.Usuarios.AnyAsync(u => u.NombreUsuario == correo || u.Correo == correo);
    }

    public async Task<Usuario?> GetUserByIdAsync(int usuarioId)
    {
        return await _applicationDbContext.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
    }

    public async Task<Usuario> RegisterAsync(Usuario usuario)
    {
        _applicationDbContext.Usuarios.Add(usuario);
        await _applicationDbContext.SaveChangesAsync();
        return usuario;
    }

    public async Task UpdatePasswordAsync(int usuarioId, string PasswordHash)
    {
        var usuario = await _applicationDbContext.Usuarios.FindAsync(usuarioId);

        if (usuario == null) return;

        usuario.Password = PasswordHash; 
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateProfileAsync(Usuario usuario)
    {
        _applicationDbContext.Usuarios.Attach(usuario);

        _applicationDbContext.Entry(usuario).Property(u => u.Nombre).IsModified = true;
        _applicationDbContext.Entry(usuario).Property(u => u.PrimerApellido).IsModified = true;
        _applicationDbContext.Entry(usuario).Property(u => u.SegundoApellido).IsModified = true;
        _applicationDbContext.Entry(usuario).Property(U => U.Telefono).IsModified = true;
        _applicationDbContext.Entry(usuario).Property(u => u.RutaFoto).IsModified = true;

        await _applicationDbContext.SaveChangesAsync(); 
    }

    public async Task<bool> UpdateUserAsync(Usuario model)
    {
        _applicationDbContext.Update(model);
        return await _applicationDbContext.SaveChangesAsync() > 0;
    }

    public async Task UpdateStateAsync(int usuarioId, bool estado)
    {
        var usuario = new Usuario
        {
            UsuarioId  = usuarioId,
            Estado = estado
        };

        _applicationDbContext.Usuarios.Attach(usuario);
        _applicationDbContext.Entry(usuario).Property(U => U.Estado).IsModified = true;

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task CreatePasswordResetTokenAsync(PasswordResetToken token)
    {
        await _applicationDbContext.PasswordResetTokens.AddAsync(token);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetPasswordResetTokenAsync(string tokenHash)
    {
        return await _applicationDbContext.PasswordResetTokens.FirstOrDefaultAsync
            (u => u.TokenHash == tokenHash && !u.Used && u.Expiration > DateTime.UtcNow);
    }

    public async Task MarkPasswordResetTokenUsedAsync(int tokenId)
    {
        var token = await _applicationDbContext.PasswordResetTokens
            .FindAsync(tokenId);

        if (token == null) return;

        token.Used = true;
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task DeletedExpiredPasswordResetTokensAsync()
    {
        var now = DateTime.Now;

        var tokens = await _applicationDbContext.PasswordResetTokens.Where(u => u.Expiration < now || u.Used).ToListAsync();

        _applicationDbContext.PasswordResetTokens.RemoveRange(tokens);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task InvalidatePasswordResetTokensAsync(int usuarioId)
    {
        var tokens = await _applicationDbContext.PasswordResetTokens
            .Where(t => t.UsuarioId == usuarioId && !t.Used).ToListAsync();

        foreach (var token in tokens)
            token.Used = true;

        await _applicationDbContext.SaveChangesAsync();
    }
    #endregion

    #region Contactos
    public async Task<List<Contacto>> GetContactsByUserIdAsync(int usuarioId)
    {
        return await _applicationDbContext.Contactos
            .Include(U => U.Detalle)
            .Where(U => U.UsuarioId == usuarioId)
            .OrderBy(u => u.Nombre)
            .ToListAsync();
    }  

    public async Task<Contacto?> GetContactByIdAsync(int contactoId)
    {
        return await _applicationDbContext.Contactos
            .Include(U => U.Detalle)
            .FirstOrDefaultAsync(U => U.ContactoId == contactoId);
    }

    public async Task<Contacto> CreateContactAsync(Contacto contacto)
    {
        _applicationDbContext.Contactos.Add(contacto);
        await _applicationDbContext.SaveChangesAsync();
        return contacto;
    }

    public async Task<bool> UpdateContactAsync(Contacto contacto)
    {
        _applicationDbContext.Contactos.Update(contacto);
        return await _applicationDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteContactAsync(int contactoId)
    {
        var contacto = await _applicationDbContext.Contactos.FirstAsync();
        if (contacto == null) return false;

        _applicationDbContext.Contactos.Remove(contacto);
        return await _applicationDbContext.SaveChangesAsync() > 0 ;
    }

    public async Task<bool> ContactExistsAsync(int contactoId)
    {
        return await _applicationDbContext.Contactos.AnyAsync(u => u.ContactoId == contactoId);
    }

    public async Task<bool> IsContactOwnerAsync(int contactoId, int usuarioId)
    {
        return await _applicationDbContext.Contactos.AnyAsync(c => c.ContactoId == contactoId && c.UsuarioId == usuarioId);
    }
    #endregion
}
