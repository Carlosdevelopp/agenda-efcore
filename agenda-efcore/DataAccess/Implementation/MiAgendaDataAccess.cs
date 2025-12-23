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

    #region GET
    public async Task<Usuario?> GetUserByCredentialAsync(string credencial)
    {
        return await _applicationDbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == credencial || u.NombreUsuario == credencial);
    } 
    
    public async Task<bool> ExistsAsync(string correo, string nombreUsuario)
    {
        return await _applicationDbContext.Usuarios.AnyAsync(u => u.NombreUsuario == correo || u.Correo == correo);
    }

    public async Task<Usuario?> GetUserByIdAsync(int usuarioId)
    {
        return await _applicationDbContext.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
    }

    public async Task<List<Contacto>> GetContactById(int usuarioId)
    {
        return await _applicationDbContext.Contactos.Include(u => u.Detalle).Where(u => u.UsuarioId == usuarioId).ToListAsync();
    }
    #endregion

    #region POST
    public async Task<Usuario> RegisterAsync(Usuario usuario)
    {
        _applicationDbContext.Usuarios.Add(usuario);
        await _applicationDbContext.SaveChangesAsync();
        return usuario;
    }
    #endregion

    #region UPDATE
    public async Task UpdatePasswordAsync(int usuarioId, string PasswordHash)
    {
        var usuario = new Usuario
        {
            UsuarioId = usuarioId,
            Password = PasswordHash,
        };

        _applicationDbContext.Usuarios.Attach(usuario);
        _applicationDbContext.Entry(usuario).Property(u => u.Password).IsModified = true;

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
    #endregion

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
}
