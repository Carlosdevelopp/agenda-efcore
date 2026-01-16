namespace Infrastructure.DTOs;

public class ActualizarContactoDto
{
    public int ContactoId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string? FotoRuta { get; set; } = string.Empty;
    public string FotoRutaAnterior { get; set; } = string.Empty;
    public string? Instagram { get; set; }  
    public string? Facebook { get; set; } 
    public string? Twitter { get; set; } 
}
