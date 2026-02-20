namespace DataAccess.Models.Tables;

public class Permiso
{
    public int PermisoId { get; set; }
    public string NombrePermiso { get; set; } = null!;
    public string Modulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<RolPermiso> RolesPermisos { get; set; } = new List<RolPermiso>();
}
