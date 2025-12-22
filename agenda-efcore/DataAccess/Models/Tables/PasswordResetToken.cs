namespace DataAccess.Models.Tables;

public class PasswordResetToken
{
    public int PasswordResetId { get; set; } 
    public int UsuarioId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public bool Used { get; set; }
    public DateTime CreatedAT { get; set; }
}
