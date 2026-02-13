namespace DataAccess.Models.Tables;

public class DetalleContactoRed
{
    public int DetContactoRedId { get; set; }
    public int ContactoId { get; set; }
    public int TipoContactoId { get; set; }
    public string URL { get; set; } = null!;
    public string NombreUsuarioRed { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }

    public virtual Contacto? Contacto { get; set; }
    public virtual TipoContacto? TipoContacto { get; set; }
}
