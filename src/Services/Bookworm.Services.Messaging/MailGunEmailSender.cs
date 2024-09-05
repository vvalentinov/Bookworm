namespace Bookworm.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bookworm.Common.Options;
    using Microsoft.Extensions.Options;
    using RestSharp;
    using RestSharp.Authenticators;

    public class MailGunEmailSender : IMailGunEmailSender
    {
        private readonly MailGunEmailSenderOptions options;

        public MailGunEmailSender(IOptions<MailGunEmailSenderOptions> mailGunOptions)
        {
            this.options = mailGunOptions.Value;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string htmlContent,
            IEnumerable<EmailAttachment> attachments = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(subject);
            ArgumentException.ThrowIfNullOrWhiteSpace(htmlContent);

            var restClientOptions = new RestClientOptions
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api", this.options.ApiKey),
            };

            var client = new RestClient(restClientOptions);

            var request = new RestRequest();
            request.AddParameter("domain", this.options.Domain, ParameterType.UrlSegment);
            request.AddParameter("from", this.options.FromEmail);
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("html", htmlContent);
            request.Resource = "{domain}/messages";
            request.Method = Method.Post;

            await client.ExecuteAsync(request);
        }
    }
}
