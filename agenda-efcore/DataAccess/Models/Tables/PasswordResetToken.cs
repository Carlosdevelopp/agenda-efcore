namespace DataAccess.Models.Tables;

public class PasswordResetToken
{
    public int PasswordResetTokenId { get; set; } 
    public int UsuarioId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public bool Used { get; set; } 
    public DateTime CreatedAt { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
