namespace Infrastructure.Contract;

public interface IEmailService
{
    Task SendPasswordResetAsync(string toEmail, string userName, string resetLink);
}
