using System.ComponentModel.DataAnnotations;

namespace MiAgendaEF.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo o nombre de usuario ess obligatorio")]
    [Display(Name = "Correo o nombre de usuario")]
    public string Credencial { get; set; } =null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}
