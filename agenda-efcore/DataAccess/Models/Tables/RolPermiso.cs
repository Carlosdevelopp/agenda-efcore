namespace DataAccess.Models.Tables;

public class RolPermiso
{
    public int RolId { get; set; }
    public int PermisoId { get; set; }
    public DateTime FechaAsignacion { get; set; }

    public virtual Rol Rol { get; set; } = null!;
    public virtual Permiso Permiso { get; set; } = null!; 
}
