using Microsoft.AspNetCore.Http;

namespace Infrastructure.DTOs;

public class RegisterUserDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;

    public IFormFile? FotoUsuario { get; set; }
}
