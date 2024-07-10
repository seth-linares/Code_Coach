using Microsoft.AspNetCore.Identity.UI.Services;
using RestSharp;
using RestSharp.Authenticators;
using SeniorProjBackend.Data;

public interface IEmailService : IEmailSender
{
    Task<bool> SendEmailAsync(string to, string subject, string body, string htmlBody = null);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly RestClient _client;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var baseUrl = _configuration["Mailgun:BaseUrl"];
        var ak = _configuration["Mailgun:AK"];

        _logger.LogInformation($"\n\n\n\nInitializing EmailService with base URL: {baseUrl}\n");
        _logger.LogInformation($"API Key: ... {ak.Substring(Math.Max(0, ak.Length - 4))}\n\n\n\n");

        var options = new RestClientOptions(baseUrl)
        {
            Authenticator = new HttpBasicAuthenticator("api", ak)
        };

        _client = new RestClient(options);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, string htmlBody = null)
    {
        var domain = _configuration["Mailgun:Domain"];
        var request = new RestRequest($"{domain}/messages", Method.Post);

        request.AddParameter("from", _configuration["Mailgun:From"]);
        request.AddParameter("to", to);
        request.AddParameter("subject", subject);
        request.AddParameter("text", body);

        if (!string.IsNullOrEmpty(htmlBody))
        {
            request.AddParameter("html", htmlBody);
        }

        try
        {
            _logger.LogInformation($"Attempting to send email to {to} with subject: {subject}");
            _logger.LogInformation($"Using domain: {domain}");
            _logger.LogInformation($"From: {_configuration["Mailgun:From"]}");

            var response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
            else
            {
                _logger.LogError($"Failed to send email. Status Code: {response.StatusCode}, Error: {response.ErrorMessage}, Content: {response.Content}");
                _logger.LogError($"Full Response: {response.Content}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while sending email to {to}");
            return false;
        }
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await SendEmailAsync(email, subject, htmlMessage, htmlMessage);
    }
}