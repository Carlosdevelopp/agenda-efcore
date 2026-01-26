using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class RegisterViewModel
{
    // --- Datos Personales del Formulario ---
    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [StringLength(30, ErrorMessage = "El nombre no puede ecceder de 30 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El primer Apellido es obligatorio.")]
    [StringLength(30, ErrorMessage = "El apellido no puede acceder de 30 caracteres")]
    [Display(Name = "Primer Apellido")]
    public string PrimerApellido { get; set; } = null!;

    [StringLength(30, ErrorMessage = "El apellido no puede ecceder de 30 caracteres")]
    [Display(Name = "Segundo Apellido")]
    public string? SegundoApellido { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    [Display(Name = "Correo Electrónico")]
    [StringLength(30)]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El Nombre de Usuario es obligatorio.")]
    [StringLength(20, ErrorMessage = "El usuario no puede exceder 20 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Solo letras, números y guión bajo")]
    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = null!;

    // --- Credenciales y Seguridad de la UI ---

    [Required(ErrorMessage = "La Contraseña es obligatoria")]
    [DataType(DataType.Password)]
    [StringLength(50, ErrorMessage = "Debe tener almenos 7 caracteres.")]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirma tu contraseña")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "El Teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Solo números, 10 dígitos")]
    [Display(Name = "Teléfono")]
    [DataType(DataType.PhoneNumber)]
    public string Telefono { get; set; } = null!;

    // --- Archivo Subido ---
    // Usar IFormFile para recibir el archivo binario
    [Display(Name = "FotoUsuario")]
    public IFormFile? FotoUsuario { get; set; }

    [Required(ErrorMessage = "Debes aceptar los términos y condiciones")]
    public bool AceptaTerminos { get; set; }
}
