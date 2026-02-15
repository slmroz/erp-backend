namespace ERP.Services.Abstractions.CommonServices;

public interface IEmailService
{
    Task SendAsync(string toEmail, string templateKey, Dictionary<string, string>? model = null);
}