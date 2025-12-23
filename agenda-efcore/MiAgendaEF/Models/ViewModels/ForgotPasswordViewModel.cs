using System.ComponentModel.DataAnnotations;

namespace Agenda.EFCore.Models.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo invalidó)")]
    public string Email { get; set; } = null!;
}
