using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Encryption;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class Judge0Controller : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<Judge0Controller> _logger;

    public Judge0Controller(IHttpClientFactory httpClientFactory, ILogger<Judge0Controller> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Judge0");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _logger = logger;
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize()
    {
        _logger.LogInformation("\n\n\nAuthorizing with Judge0\n\n\n");
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "authorize");
            _logger.LogInformation($"Request URI: {request.RequestUri}");
            _logger.LogInformation($"Request Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))}");

            var response = await _httpClient.SendAsync(request);
            _logger.LogInformation($"Response Status Code: {response.StatusCode}");

            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\nError authorizing with Judge0\n\n\n");
            return StatusCode(500, "An error occurred while authorizing.");
        }
    }

    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            return Ok($"Judge0 is reachable. Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unable to reach Judge0: {ex.Message}");
        }
    }

    [HttpPost("submissions")]
    public async Task<IActionResult> CreateSubmission(SubmissionRequestDto requestDto)
    {
        _logger.LogInformation("\n\n\n\nCreating Submission\n\n\n\n");
        try
        {
            var response = await _httpClient.PostAsJsonAsync("submissions", requestDto);
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError creating submission\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("submissions/{token}")]
    public async Task<IActionResult> GetSubmission(string token)
    {
        _logger.LogInformation("\n\n\n\nGetting Submission\n\n\n\n");
        try
        {
            var response = await _httpClient.GetAsync($"submissions/{token}");
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError getting submission\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages()
    {
        _logger.LogInformation("\n\n\n\nGetting Languages\n\n\n\n");
        try
        {
            var response = await _httpClient.GetAsync("languages");
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError getting languages\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses()
    {
        _logger.LogInformation("\n\n\n\nGetting Statuses\n\n\n\n");
        try
        {
            var response = await _httpClient.GetAsync("statuses");
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError getting statuses\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    private async Task<IActionResult> FormatResponse(HttpResponseMessage response)
    {
        _logger.LogInformation("\n\n\nFormatting Response\n\n\n");
        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation($"Response Status Code: {response.StatusCode}");
        _logger.LogInformation($"Response Content: {content}");

        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Ok("Request successful, but no content returned.");
            }

            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                var formattedJson = JsonSerializer.Serialize(jsonElement, _jsonOptions);
                return Content(formattedJson, "application/json");
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Response is not in JSON format. Returning raw content.");
                return Content(content, response.Content.Headers.ContentType?.ToString() ?? "text/plain");
            }
        }

        _logger.LogWarning($"\n\n\nNon-success status code: {response.StatusCode}\nResponse content: {content}\n\n\n");
        return StatusCode((int)response.StatusCode, content);
    }
}