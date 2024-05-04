namespace Bookworm.Services.Messaging
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendPasswordResetEmailAsync(
            string toName,
            string toEmail,
            string callbackUrl);

        Task SendEmailConfirmationAsync(
            string toName,
            string toEmail,
            string callbackUrl);

        Task SendBookApprovedEmailAsync(
            string toName,
            string toEmail,
            string bookTitle);
    }
}
