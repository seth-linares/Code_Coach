using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.Middleware
{
    public interface IEmailService
    {
        Task<bool> TrySendEmailAsync(string to, string subject, string body, string? htmlBody = null);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly RestClient _client;
        private readonly IOptions<EmailServiceOptions> _options;

        public EmailService(IOptions<EmailServiceOptions> mailGunOptions, ILogger<EmailService> logger)
        {
            _options = mailGunOptions;
            _logger = logger;

            _logger.LogInformation("\n\n\n\nInitializing EmailService with base URL: {baseUrl}\n\n\n\n", _options.Value.BaseURL);

            var restClientOptions = new RestClientOptions(mailGunOptions.Value.BaseURL)
            {
                Authenticator = new HttpBasicAuthenticator("api", mailGunOptions.Value.AK)
            };

            _client = new RestClient(restClientOptions);
        }

        public async Task<bool> TrySendEmailAsync(string to, string subject, string body, string? htmlBody = null)
        {
            var request = new RestRequest($"{_options.Value.Domain}/messages", Method.Post);

            request.AddParameter("from", _options.Value.From);
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("text", body);

            if (!string.IsNullOrEmpty(htmlBody))
            {
                request.AddParameter("html", htmlBody);
            }

            try
            {

                var response = await _client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Email sent successfully to {to}", to);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email to {to}", to);
                return false;
            }
        }

    }

    public class EmailServiceOptions
    {
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string BaseURL { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string AK { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Domain { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string From { get; set; }
    }
}