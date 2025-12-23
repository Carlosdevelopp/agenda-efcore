using Infrastructure.Contract;

namespace Infrastructure.Services;

public class EmailService : IEmailService  
{
    public async Task SendAsync(string to, string subject, string body)
    {
        await Task.CompletedTask;
    }
}
