using Microsoft.AspNetCore.Http;

namespace Infrastructure.DTOs;

public class RegisterUserDTO
{
    public string Nombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string Correo { get; set; }
    public string Password { get; set; }
    public string NombreUsuario { get; set; }
    public string Telefono { get; set; }

    public IFormFile? FotoUsuario { get; set; }
}
