namespace MiAgendaEF.Models.ViewModels;

public class AgendaViewModel
{
    public string Titulo { get; set; } = "Mis Contactos"; 
    public int TotalContactos { get; set; }
    public string? TerminoBusqueda { get; set; } = null!;

    //Lista de ViewModels
    public virtual List<ContactoViewModel> Contactos { get; set; } = new List<ContactoViewModel>();

}
