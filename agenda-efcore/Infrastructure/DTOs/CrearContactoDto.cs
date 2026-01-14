namespace Infrastructure.DTOs;

public class CrearContactoDto
{
    public string? NombreCompleto { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? FotoRuta { get; set; }  
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }
}
