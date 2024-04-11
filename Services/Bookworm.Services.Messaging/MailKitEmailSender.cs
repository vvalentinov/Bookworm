namespace Bookworm.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MailKit.Net.Smtp;
    using MimeKit;

    public class MailKitEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(
            string fromEmail,
            string fromName,
            string toEmail,
            string toName,
            string subject,
            string htmlContent,
            string appPassword = null,
            IEnumerable<EmailAttachment> attachments = null)
        {
            if (string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException();
            }

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
            mailMessage.To.Add(new MailboxAddress(toName, toEmail));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("html")
            {
                Text = htmlContent,
            };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync("smtp.gmail.com", 465, true);
            await smtpClient.AuthenticateAsync(fromEmail, appPassword);
            await smtpClient.SendAsync(mailMessage);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
