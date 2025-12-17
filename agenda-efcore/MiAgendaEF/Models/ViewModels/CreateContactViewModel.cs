using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class CreateContactViewModel
{
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

    [Display(Name = "Foto")]
    public IFormFile? FotoArchivo { get; set; }

    [Required(ErrorMessage = "El Teléfono es obligatorio.")]
    [Display(Name = "Teléfono")]
    [DataType(DataType.PhoneNumber)]
    public string Telefono { get; set; } = null!;

    // La lista de las redes sociales que el usuario está agregando (los datos de entrada)
    public List<RedSocialViewModel> RedesSociales { get; set; } = new List<RedSocialViewModel>();

    // La lista de OPCIONES para el Dropdown (Se llena en el Controlador)
    public IEnumerable<SelectListItem> RedesDisponibles { get; set; } = new List<SelectListItem>();

    // Campo necesario para vincular el contacto con el usuario logueado
    public int UsuarioPropietarioId { get; set; }
}
