namespace DataAccess.Models.Tables;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = null!;
    public string PrimerApellido { get; set; } = null!;
    public string? SegundoApellido { get; set; } 
    public string Correo { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? RutaFoto { get; set; } 
    public string Telefono { get; set; } = null!;
    public DateTime FechaRegistro { get; set; } 
    public bool Estado { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public DateTime? FechaAceptacionTerminos { get; set; }
}
