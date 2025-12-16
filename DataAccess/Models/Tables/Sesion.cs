namespace DataAccess.Models.Tables;

public class Sesion
{
    public int SesionId { get; set; }
    public int UsuarioId { get; set; }
    public string TokenSesion { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public  DateTime FechaExpiracion { get; set; }
    public string IpAddress { get; set; } = null!;
    public string Navegador { get; set; } = null!;
    public bool Estado { get; set; }
}
