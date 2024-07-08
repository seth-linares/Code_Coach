using RestSharp;
public interface IEmailService
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
        var apiKey = _configuration["Mailgun:ApiKey"];
        _client = new RestClient(baseUrl);
        _client.AddDefaultHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"api:{apiKey}"))}");
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, string htmlBody = null)
    {
        var domain = _configuration["Mailgun:Domain"];
        var request = new RestRequest($"{domain}/messages", Method.Post);

        request.AddParameter("from", _configuration["Mailgun:From"]);
        request.AddParameter("to", to);
        request.AddParameter("subject", subject);
        request.AddParameter("text", body);

        // Add HTML content if provided
        if (!string.IsNullOrEmpty(htmlBody))
        {
            request.AddParameter("html", htmlBody);
        }

        try
        {
            var response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
            else
            {
                _logger.LogError($"Failed to send email. Status Code: {response.StatusCode}, Error: {response.ErrorMessage}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while sending email to {to}");
            return false;
        }
    }
}