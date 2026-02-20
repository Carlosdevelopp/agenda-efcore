namespace DataAccess.Models.Tables;

public class Rol
{
    public int RolId { get; set; }
    public string NombreRol { get; set; } = null!;

    public virtual ICollection<UsuarioRol> UsuariosRoles { get; set; } = new List<UsuarioRol>();
    public virtual ICollection<RolPermiso> RolesPermisos { get; set; } = new List<RolPermiso>();
}
