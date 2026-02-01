namespace MiAgendaEF.Models.ViewModels;

public class RedSocialViewModel
{
    //Para Update
    public int DetalleContactoRedId { get; set; } 
    //Instagram=1, Facebook=2, Twitter=3
    public int TipoContactoId { get; set; }
    //"Instagram","Facebook", etc.
    public string? NombreRed { get; set; }
    public string? URL { get; set; } 
    public int ContactoId { get; set; }

}
