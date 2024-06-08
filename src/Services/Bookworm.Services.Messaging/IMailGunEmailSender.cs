namespace Bookworm.Services.Messaging
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMailGunEmailSender
    {
        Task SendEmailAsync(
            string to,
            string subject,
            string htmlContent,
            IEnumerable<EmailAttachment> attachments = null);
    }
}
