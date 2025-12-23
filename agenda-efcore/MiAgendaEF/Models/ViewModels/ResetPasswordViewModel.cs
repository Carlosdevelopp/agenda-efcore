using System.ComponentModel.DataAnnotations;

namespace Agenda.EFCore.Models.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Las  contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = null!;
}
