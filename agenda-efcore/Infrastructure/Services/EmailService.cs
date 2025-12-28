using Infrastructure.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    private string BuildBaseUrl()
    {
        // Producción
        var baseUrl = _configuration["AppSettings:BaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
            return baseUrl;

        // Local
        var request = _httpContextAccessor.HttpContext?.Request
                      ?? throw new InvalidOperationException("HttpContext no disponible");

        return $"{request.Scheme}://{request.Host}";
    }

    public async Task SendPasswordResetAsync(string toEmail, string userName, string token)
    {
        var smtp = _configuration["EmailSettings:Smtp"];
        var port = int.Parse(_configuration["EmailSettings:Port"]!);
        var from = _configuration["EmailSettings:From"];
        var user = _configuration["EmailSettings:EMAIL_USER"];
        var password = _configuration["EmailSettings:EMAIL_PASSWORD"];

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            throw new Exception("Credenciales de correo no configuradas");

        var baseUrl = BuildBaseUrl();
        var resetLink = $"{baseUrl}/Account/ResetPassword?token={token}";

        var body = $@"<p>Hola <strong>{userName}</strong>,</p>
                      <p>Haz clic en el siguiente enlace para restablecertu contraseña:</p>
                      <p><a href='{resetLink}'>{resetLink}</a></p>
                      <p>Este enlace expira en 1 hora.</p>
        ";  

        var message = new MailMessage
        {
            From = new MailAddress(from!),
            Subject = "Restablecer contraseña",
            Body = body,
            IsBodyHtml = true
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
