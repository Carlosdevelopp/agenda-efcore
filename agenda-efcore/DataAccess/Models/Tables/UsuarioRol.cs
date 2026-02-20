namespace DataAccess.Models.Tables;

public class UsuarioRol
{
    public int UsuarioId { get; set; }
    public int RolId { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Rol Rol { get; set; } = null!;
}
