using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class ContactoViewModel
{
    public int ContactoId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [Display(Name = "Nombre Completo")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El primer apellido es requerido")]
    [Display(Name = "Primer apellido")]
    public string PrimerApellido { get; set; } = null!;

    [Display(Name = "Segundo apellido")]
    public string? SegundoApellido { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    [Display(Name = "Fecha de Nacimiento")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string Telefono { get; set; } = null!;

    public int? Edad { get; set; }

    public string? FotoRuta { get; set; } = string.Empty;

    [Display(Name="Foto de perfil")]
    public  IFormFile? FotoPerfil { get; set; }

    public int UsuarioId { get; set; }

    //public int UsuarioId { get; set; }
    public List<RedSocialViewModel> RedesSociales { get; set; } = new List<RedSocialViewModel>();
}
