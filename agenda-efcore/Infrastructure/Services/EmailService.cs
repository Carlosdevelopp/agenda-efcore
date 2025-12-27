using Infrastructure.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
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
        var smtp = _configuration["EmailSettings:Smtp"];
        var port = int.Parse(_configuration["EmailSettings:Port"]!);
        var from = _configuration["EmailSettings:From"];
        var user = _configuration["EmailSettings:EMAIL_USER"];
        var password = _configuration["EmailSettings:EMAIL_PASSWORD"];

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            throw new Exception("Credenciales de correo no configuradas");

        var message = new MailMessage
        {
            From = new MailAddress(from!),
            Subject = "Restablecer contraseña",
            Body = $@" Hola {userName}, Haz clic en el siguiente enlace para restablecer tu contraseña: {resetLink} Este enlace expira en 1 hora.",
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(smtp, port)
        {
            Credentials = new NetworkCredential(user, password.Replace(" ", "")),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
