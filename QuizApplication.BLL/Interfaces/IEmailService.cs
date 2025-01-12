using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
        Task SendEmailTemplateAsync(string to, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    }
}
