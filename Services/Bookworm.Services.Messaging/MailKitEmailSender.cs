namespace Bookworm.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Options;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Options;
    using MimeKit;

    using static Bookworm.Common.Constants.GlobalConstants;

    public class MailKitEmailSender : IEmailSender
    {
        private readonly MailKitEmailSenderOptions mailKitOptions;

        public MailKitEmailSender(IOptions<MailKitEmailSenderOptions> mailKitOptions)
        {
            this.mailKitOptions = mailKitOptions.Value;
        }

        public async Task SendBookApprovedEmailAsync(
            string toName,
            string toEmail,
            string bookTitle)
        {
            await this.ConstructAndSendEmailAsync(
                toEmail,
                toName,
                "Approved Book",
                $"<h1>Your book: {bookTitle} has been approved! Congratulations!</h1>");
        }

        public async Task SendPasswordResetEmailAsync(
            string toName,
            string toEmail,
            string callbackUrl)
        {
            await this.ConstructAndSendEmailAsync(
                toEmail,
                toName,
                "Password Reset",
                $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");
        }

        public async Task SendEmailConfirmationAsync(
            string toName,
            string toEmail,
            string callbackUrl)
        {
            await this.ConstructAndSendEmailAsync(
                toEmail,
                toName,
                "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
        }

        private async Task ConstructAndSendEmailAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlContent,
            IEnumerable<EmailAttachment> attachments = null)
        {
            if (string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("Subject and message should be provided.");
            }

            var fromEmail = this.mailKitOptions.Email;
            var appPassword = this.mailKitOptions.AppPassword;

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(SystemName, fromEmail));
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
