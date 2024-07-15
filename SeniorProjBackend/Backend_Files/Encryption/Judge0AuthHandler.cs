namespace SeniorProjBackend.Encryption
{
    public class Judge0AuthHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Judge0AuthHandler> _logger;

        public Judge0AuthHandler(IConfiguration configuration, ILogger<Judge0AuthHandler> logger)
        {
            _configuration = configuration;
            _logger=logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = _configuration["Judge0:AuthToken"];
            request.Headers.Add("X-Judge0-User", token);
            _logger.LogInformation($"\n\n\n\nAdded X-Judge0-User header with token: {token.Substring(0, 5)}...\n\n\n\n");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
