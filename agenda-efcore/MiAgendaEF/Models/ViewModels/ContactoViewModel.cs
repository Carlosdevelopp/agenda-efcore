using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class ContactoViewModel
{
    public int ContactoId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    [Display(Name = "Fecha de Nacimiento")]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string Telefono { get; set; } = null!;

    public int? Edad { get; set; }

    public string? FotoRuta { get; set; }

    [Display(Name="Foto de perfil")]
    public  IFormFile? FotoPerfil { get; set; }

    public int UsuarioId { get; set; }

    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }

    //public int UsuarioId { get; set; }
    public List<RedSocialViewModel> RedesSociales { get; set; } = new List<RedSocialViewModel>();
    public IEnumerable<SelectListItem> RedesDisponibles { get; set; } = new List<SelectListItem>();
}
