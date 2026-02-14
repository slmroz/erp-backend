using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace ERP.Infrastructure.CommonServices;
public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly Dictionary<string, EmailTemplateConfig> _templates;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _templates = _configuration.GetSection("EmailTemplates").Get<Dictionary<string, EmailTemplateConfig>>()
            ?? new Dictionary<string, EmailTemplateConfig>();
    }

    public async Task SendAsync(string toEmail, string templateKey, object? model = null)
    {
        if (!_templates.TryGetValue(templateKey, out var template))
        {
            throw new ArgumentException($"Template '{templateKey}' not found");
        }

        var subject = RenderTemplate(template.Subject, model);
        var body = RenderTemplate(template.Body, model);

        var smtpSettings = _configuration.GetSection("EmailSettings");
        var smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
        {
            Port = int.Parse(smtpSettings["SmtpPort"] ?? "587"),
            Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                ),
            EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true")
        };
        var mailMessage = new MailMessage();
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.To.Add(toEmail);
        mailMessage.IsBodyHtml = true;

        await smtpClient.SendMailAsync(mailMessage);
    }

    private static string RenderTemplate(string template, object? model)
    {
        if (model == null) return template;
        // Simple RazorLight or Handlebars.NET rendering
        // For demo: basic string interpolation (enhance with RazorLight nuget)
        return template.Replace("{{UserName}}", model.GetType().GetProperty("UserName")?.GetValue(model)?.ToString() ?? "");
    }
}