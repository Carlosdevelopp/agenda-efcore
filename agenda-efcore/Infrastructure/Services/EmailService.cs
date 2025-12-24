using Infrastructure.Contract;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetAsync(string toEmail, string userName, string resetLink)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:From"]!),
            Subject = "Restablecer contraseña",
            Body = $@" Hola {userName}, Haz clic en el siguiente enlace para resttablecer tu contraseña: 
            {resetLink} Este enlace expirá en 1 hora.",
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        using var smtp = new SmtpClient(_configuration["EmailSettings:Smtp"],
            int.Parse(_configuration["EmailSettings:Port"]!))
        {
            Credentials = new NetworkCredential(
                _configuration["EmailSettings:User"],
                _configuration["EmailSettings:Password"]),
            EnableSsl = true
        };

        await smtp.SendMailAsync(message);
    }
}
