using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.Encryption
{
    public class Judge0AuthHandler : DelegatingHandler
    {
        private readonly IOptions<Judge0Options> _options;
        private readonly ILogger<Judge0AuthHandler> _logger;

        public Judge0AuthHandler(IOptions<Judge0Options> options, ILogger<Judge0AuthHandler> logger)
        {
            _options = options;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("\n\n\n\nAttempting to send problem to Judge0\n\n\n\n");

            request.Headers.Add("x-rapidapi-key", _options.Value.RapidApiKey);
            request.Headers.Add("x-rapidapi-host", _options.Value.RapidApiHost);

            return await base.SendAsync(request, cancellationToken);
        }
    }

    public class Judge0Options
    {
        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string RapidApiKey { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string RapidApiHost { get; set; }
    }
}
