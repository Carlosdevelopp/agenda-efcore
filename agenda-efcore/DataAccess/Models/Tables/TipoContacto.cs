namespace DataAccess.Models.Tables;

public class TipoContacto
{
    public int TipoContactoId { get; set; }
    public string NombreTipo { get; set; } = null!;
    public string Icono { get; set; } = null!;

    public virtual ICollection<DetalleContactoRed> Detalles { get; set; }
}
