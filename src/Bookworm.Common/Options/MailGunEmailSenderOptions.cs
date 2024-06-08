namespace Bookworm.Common.Options
{
    public class MailGunEmailSenderOptions
    {
        public const string MailGunEmailSender = "MailGunEmailSender";

        public string ApiKey { get; set; }

        public string Domain { get; set; }

        public string FromEmail { get; set; }
    }
}
