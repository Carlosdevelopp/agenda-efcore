namespace DataAccess.Models.Tables;

public class PasswordResetToken
{
    public int PasswordResetTokenId { get; set; } 
    public int UsuarioId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public bool Used { get; set; } = false;
    public DateTime CreatedAT { get; set; }

    public virtual Usuario? usuario { get; set; } = null!;
}
