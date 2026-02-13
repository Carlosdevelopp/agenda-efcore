using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class ContactSocialViewModel
{
    public int ContactoId { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El Primer Apellido es obligatorio.")]
    [Display(Name = "Apellido Paterno")]
    public string PrimerApellido { get; set; } = null!;

    [Display(Name = "Apellido Materno")]
    public string? SegundoApellido { get; set; } 

    [Required(ErrorMessage = "La Fecha de Nacimiento es obligatoria.")]
    [Display(Name = "Fecha de Nacimiento")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    [Display(Name = "Foto de perfil")]
    public IFormFile? FotoPerfil { get; set; }

    public string? FotoRuta { get; set; }

    [Required(ErrorMessage = "El Teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Formato de teléfono invalido.")]
    [Display(Name = "Teléfono")]
    [DataType(DataType.PhoneNumber)]
    public string Telefono { get; set; } = null!;

    [RegularExpression(@"^[A-Za-z0-9_.]{1,30}$", ErrorMessage = "Usuario de Instagram no válido")]
    public string? Instagram { get; set; }

    [RegularExpression(@"^[A-Za-z0-9.]{5,50}$", ErrorMessage = "Usuario de Facebook no válido")]
    public string? Facebook { get; set; }

    [RegularExpression(@"^[A-Za-z0-9_]{1,15}$", ErrorMessage = "Usuario de Twitter no válido")]
    public string? Twitter { get; set; }

    public int UsuarioId { get; set; }
}
