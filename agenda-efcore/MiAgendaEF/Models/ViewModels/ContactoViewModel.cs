namespace MiAgendaEF.Models.ViewModels;

public class ContactoViewModel
{
    public int ContactoId { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public int Edad { get; set; }
    public string Telefono { get; set; } = null!;
    public int UsuarioId { get; set; }
    public List<RedSocialViewModel> RedesSociales { get; set; } = new List<RedSocialViewModel>();
}
