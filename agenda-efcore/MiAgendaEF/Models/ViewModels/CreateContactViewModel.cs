using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class CreateContactViewModel
{
    public int ContactoId { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El Primer Apellido es obligatorio.")]
    [Display(Name = "Apellido Paterno")]
    public string PrimerApellido { get; set; } = null!;

    [Display(Name = "Apellido Materno")]
    public string? SegundoApellido { get; set; } = null!;

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

    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }

    public int UsuarioId { get; set; }
}
