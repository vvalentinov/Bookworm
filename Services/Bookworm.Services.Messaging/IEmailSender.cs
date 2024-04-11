namespace Bookworm.Services.Messaging
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(
            string fromEmail,
            string fromName,
            string toEmail,
            string toName,
            string subject,
            string htmlContent,
            string appPassword = null,
            IEnumerable<EmailAttachment> attachments = null);
    }
}
