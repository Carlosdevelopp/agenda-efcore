namespace Infrastructure.DTOs;

public class ActualizarContactoDto
{
    public int ContactoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string SegundoApellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string? FotoRuta { get; set; } = string.Empty;
    public string FotoRutaAnterior { get; set; } = string.Empty;
    public string? Instagram { get; set; }  
    public string? Facebook { get; set; } 
    public string? Twitter { get; set; } 
}
