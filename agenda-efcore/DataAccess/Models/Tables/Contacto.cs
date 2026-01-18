namespace DataAccess.Models.Tables;

public class Contacto
{
    public int ContactoId { get; set; }
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = null!;
    public string PrimerApellido { get; set; } = null!;
    public string? SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; } 
    public string? FotoRuta { get; set; } 
    public DateTime FechaRegistro { get; set; }
    public string Telefono { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
    public virtual ICollection<DetalleContactoRed> Detalle { get; set; } = new List<DetalleContactoRed>();
}
