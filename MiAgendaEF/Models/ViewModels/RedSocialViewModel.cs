using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class RedSocialViewModel
{
    // ID para el tipo de red (ej. 1=Facebook, 2=Instagram). 
    // Esto se selecciona desde un dropdown en la vista.
    [Required]
    public int TipoContactoId { get; set; }

    public string NombreTipo { get; set; } = null!;

    [Required]
    [Display(Name = "Usuario / URL")]
    public string URL { get; set; } = null!;

    public string NombreUsuarioRed { get; set; } = null!;
}
