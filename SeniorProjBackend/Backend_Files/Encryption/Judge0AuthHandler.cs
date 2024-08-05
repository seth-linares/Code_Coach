namespace SeniorProjBackend.Encryption
{
    public class Judge0AuthHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Judge0AuthHandler> _logger;

        public Judge0AuthHandler(IConfiguration configuration, ILogger<Judge0AuthHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("\n\n\n\nAttempting to send problem to Judge0\n\n\n\n");
            string rapidApiKey = _configuration["Judge0:RapidApiKey"];
            string rapidApiHost = _configuration["Judge0:RapidApiHost"];

            request.Headers.Add("x-rapidapi-key", rapidApiKey);
            request.Headers.Add("x-rapidapi-host", rapidApiHost);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
