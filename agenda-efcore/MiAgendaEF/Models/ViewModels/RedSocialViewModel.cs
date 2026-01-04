using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class RedSocialViewModel
{
    // ID para el tipo de red (ej. 1=Facebook, 2=Instagram). 
    // Esto se selecciona desde un dropdown en la vista.
    [Required]

    public string NombreTipo { get; set; } = null!;

    [Required]
    [Url(ErrorMessage = "Debe ser una URL válida")]
    [Display(Name = "Usuario / URL")]
    public string URL { get; set; } = null!;
    public int TipoContactoId { get; set; }

    public string NombreUsuarioRed { get; set; } = null!;
}
