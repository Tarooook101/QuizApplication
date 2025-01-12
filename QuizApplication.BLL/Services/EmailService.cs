using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = CreateSmtpClient();
                using var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                await client.SendMailAsync(message, cancellationToken);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw new ApplicationException("Failed to send email", ex);
            }
        }

        public async Task SendEmailTemplateAsync(string to, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            var template = await LoadTemplateAsync(templateName);
            var body = RenderTemplate(template, parameters);
            await SendEmailAsync(to, parameters["Subject"], body, cancellationToken);
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.Port,
                Credentials = new System.Net.NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            // In a real application, this would load from file/database
            // For now, returning a simple HTML template
            return """
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8" />
                </head>
                <body>
                    <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                        {{Content}}
                    </div>
                </body>
                </html>
                """;
        }

        private string RenderTemplate(string template, Dictionary<string, string> parameters)
        {
            return parameters.Aggregate(template, (current, param) =>
                current.Replace($"{{{{{param.Key}}}}}", param.Value));
        }
    }
}
