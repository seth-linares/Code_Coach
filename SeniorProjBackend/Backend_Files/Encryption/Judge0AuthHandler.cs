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
            string rapidApiKey = _configuration["Judge0:RapidApiKey"];
            string rapidApiHost = _configuration["Judge0:RapidApiHost"];

            request.Headers.Add("x-rapidapi-key", rapidApiKey);
            request.Headers.Add("x-rapidapi-host", rapidApiHost);

            _logger.LogInformation($"\n\n\n\nAdding headers to Judge0 request:\nx-rapidapi-key: {rapidApiKey}\nx-rapidapi-host: {rapidApiHost}\n\n\n\n");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
