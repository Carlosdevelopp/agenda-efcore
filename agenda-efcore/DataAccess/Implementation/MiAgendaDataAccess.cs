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
    
    public async Task<bool> ExistsAsync(string correo, string nombreeUsuario)
    {
        return await _applicationDbContext.Usuarios.AnyAsync(u => u.NombreUsuario == correo || u.Correo == correo);
    }

    public async Task<Usuario> CreateUserAsync(Usuario usuario)
    {
        _applicationDbContext.Usuarios.Add(usuario);
        await _applicationDbContext.SaveChangesAsync();
        return usuario;
    }

    //public async Task AddUserAsync(Usuario usuario)
    //{
    //    _applicationDbContext.Usuarios.Add(usuario);
    //    await _applicationDbContext.SaveChangesAsync();
    //}

    public async Task<List<Contacto>> GetContactById(int usuarioId)
    {
        return await _applicationDbContext.Contactos.Include(u => u.Detalle).Where(u => u.UsuarioId == usuarioId).ToListAsync();
    }

    public async Task<List<Usuario>> GetAllUsersAsync()
    {
        var usuarios = await _applicationDbContext.Usuarios.ToListAsync();
        return usuarios;
    }
    #endregion
}
