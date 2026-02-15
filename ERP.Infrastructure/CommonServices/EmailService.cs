using ERP.Services.Abstractions.CommonServices;
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

    public async Task SendAsync(string toEmail, string templateKey, Dictionary<string, string>? model = null)
    {
        if (!_templates.TryGetValue(templateKey, out var template))
        {
            throw new ArgumentException($"Template '{templateKey}' not found");
        }

        var subject = RenderTemplate(template.Subject, model);
        var body = RenderTemplate(template.Body, model);

        var smtpSettings = _configuration.GetSection("EmailSettings");
        var fromEmail = smtpSettings["FromEmail"];


        var smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
        {
            Port = int.Parse(smtpSettings["SmtpPort"] ?? "587"),
            Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                ),
            EnableSsl = false //bool.Parse(smtpSettings["EnableSsl"] ?? "true")
        };
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(fromEmail);
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.To.Add(toEmail);
        mailMessage.IsBodyHtml = true;

        await smtpClient.SendMailAsync(mailMessage);
    }

    private static string RenderTemplate(string template, Dictionary<string, string>? model)
    {
        if (model == null) return template;

        foreach (var item in model)
        {
            template = template.Replace("{{" + item.Key + "}}", item.Value);
        }

        return template;
    }
}