using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class RegisterViewModel
{
    // --- Datos Personales del Formulario ---
    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El primer Apellido es obligatorio.")]
    [Display(Name = "Primer Apellido")]
    public string PrimerApellido { get; set; } = null!;

    [Display(Name = "Segundo Apellido")]
    public string? SegundoApellido { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress]
    [Display(Name = "Correo Electrónico")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El Nombre de Usuario es obligatorio.")]
    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = null!;

    // --- Credenciales y Seguridad de la UI ---

    [Required(ErrorMessage = "La Contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "Debe tener almenos 7 caracteres.")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "El Teléfono  es obligatorio.")]
    [Display(Name = "Teléfono")]
    [DataType(DataType.PhoneNumber)]
    public string Telefono { get; set; } = null!;

    // --- Archivo Subido ---
    // Usar IFormFile para recibir el archivo binario
    [Display(Name = "FotoUsuario")]
    public IFormFile? FotoUsuario { get; set; }
}
