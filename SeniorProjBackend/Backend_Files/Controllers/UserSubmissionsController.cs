using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using System.Text;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserSubmissionsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly OurDbContext _context;
    private readonly ILogger<UserSubmissionsController> _logger;
    private readonly UserManager<User> _userManager;

    public UserSubmissionsController(IHttpClientFactory httpClientFactory, ILogger<UserSubmissionsController> logger, UserManager<User> userManager, OurDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient("Judge0");
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    // POST: https://www.codecoachapp.com/api/UserSubmissions/SubmitCode
    [HttpPost("SubmitCode")]
    public async Task<IActionResult> SubmitCode(SubmissionRequestDto submissionRequest)
    {

        _logger.LogInformation("\n\n\n\nTRYING TO SUBMIT\nJ0Lang ID: {Judge0LanguageId}\nProblem ID: {ProblemId}\n\n\n\n", submissionRequest.Judge0LanguageId, submissionRequest.ProblemId);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }



        var problem = await _context.Problems
            .Include(p => p.ProblemLanguages)
            .ThenInclude(pl => pl.Language)
            .FirstOrDefaultAsync(p => p.ProblemID == submissionRequest.ProblemId);

        if (problem == null)
        {
            return NotFound("Problem not found");
        }

        if (problem.ProblemLanguages == null)
        {
            return BadRequest("Problem languages not configured for this problem");
        }

        var problemLanguage = problem.ProblemLanguages
            .FirstOrDefault(pl => pl.Language != null && pl.Language.Judge0ID == submissionRequest.Judge0LanguageId);

        if (problemLanguage == null)
        {
            return BadRequest("Invalid language for this problem");
        }

        string combinedCode = CombineCode(submissionRequest.EncodedCode, problemLanguage.TestCode, submissionRequest.Judge0LanguageId);

        var judge0Request = new
        {
            language_id = submissionRequest.Judge0LanguageId,
            source_code = combinedCode
        };

        var response = await _httpClient.PostAsJsonAsync("submissions?base64_encoded=true&wait=true", judge0Request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("\n\n\n\nJudge0 API error: {response.StatusCode}\n\n\n\n", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error submitting code to Judge0");
        }

        var submissionResult = await response.Content.ReadFromJsonAsync<SubmissionResult>();

        if (submissionResult == null)
        {
            return StatusCode(500, "Error processing Judge0 response");
        }
        _logger.LogInformation("\n\n\n\nCOMPILE ERROR: {submissionResult.CompileOutput}\n\n\n\n", submissionResult.CompileOutput);
        _logger.LogInformation("\n\n\n\nSTD ERROR: {submissionResult.Stderr}\n\n\n\n", submissionResult.Stderr);

        bool isSuccessful = IsSubmissionSuccessful(submissionResult);

        var userSubmission = new UserSubmission
        {
            UserId = user.Id,
            ProblemID = problem.ProblemID,
            LanguageID = problemLanguage.LanguageID,
            SubmittedCode = submissionRequest.EncodedCode,
            SubmissionTime = DateTimeOffset.UtcNow,
            IsSuccessful = isSuccessful,
            Token = submissionResult.Token,
            ExecutionTime = submissionResult.Time,
            MemoryUsed = submissionResult.Memory
        };

        _context.UserSubmissions.Add(userSubmission);

        await UpdateUserStatistics(user, problem.Points, isSuccessful);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            IsSuccessful = isSuccessful,
            Stdout = submissionResult.Stdout,
            Stderr = submissionResult.Stderr,
            CompileOutput = submissionResult.CompileOutput,
            ExecutionTime = submissionResult.Time,
            MemoryUsed = submissionResult.Memory,
            Status = submissionResult.Status
        });
    }

    private string CombineCode(string userCode, string testCode, int languageId)
    {

        _logger.LogInformation("\n\n\n\nCOMBINING THE CODE\n\n\n\n");
        // Assuming both userCode and testCode are already Base64 encoded
        byte[] userCodeBytes = Convert.FromBase64String(userCode);
        byte[] testCodeBytes = Convert.FromBase64String(testCode);

        string decodedUserCode = Encoding.UTF8.GetString(userCodeBytes);
        string decodedTestCode = Encoding.UTF8.GetString(testCodeBytes);

        string combinedCode = languageId switch
        {
            92 => decodedUserCode + "\n" + decodedTestCode, // Python
            _ => decodedTestCode + "\n" + decodedUserCode   // C# and C++
        };

        var combinedEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(combinedCode));

        _logger.LogInformation("\n\n\n\nCombined encoded: {combinedEncoded}\n\n\n\n", combinedEncoded);

        return combinedEncoded;
    }

    private bool IsSubmissionSuccessful(SubmissionResult result)
    {
        if (string.IsNullOrEmpty(result.Stdout))
        {
            return false;
        }

        byte[] outputBytes = Convert.FromBase64String(result.Stdout);
        string decodedOutput = Encoding.UTF8.GetString(outputBytes).Trim();

        // Split the output into lines and get the last line
        string[] outputLines = decodedOutput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        string lastLine = outputLines.LastOrDefault()?.Trim();

        bool zeroErrors = lastLine == "Failed: 0";

        _logger.LogInformation("\n\n\n\nLast line is 'Failed: 0': {zeroErrors}\nLast line: '{lastLine}'\n\n\n\n", zeroErrors, lastLine);

        return zeroErrors;
    }

    private async Task UpdateUserStatistics(User user, int points, bool isSuccessful)
    {
        user.AttemptedProblems++;
        if (isSuccessful)
        {
            user.CompletedProblems++;
            user.TotalScore += points;
        }
        await _userManager.UpdateAsync(user);
    }


    [HttpPost("GetSuccessfulSubmissions")]
    public async Task<IActionResult> GetSuccessfulSubmissions()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var query = _context.UserSubmissions
            .Where(s => s.UserId == user.Id && s.IsSuccessful)
            .Include(s => s.Problem)
            .Include(s => s.Language)
            .OrderByDescending(s => s.SubmissionTime);

        var totalCount = await query.CountAsync();

        var successfulSubmissions = await query
            .Select(s => new SuccessfulSubmissionDto
            {
                SubmissionId = s.SubmissionID,
                ProblemId = s.ProblemID,
                ProblemTitle = s.Problem.Title,
                SubmittedCode = s.SubmittedCode,
                LanguageName = s.Language.Name,
                SubmissionTime = s.SubmissionTime,
                ExecutionTime = s.ExecutionTime,
                MemoryUsed = s.MemoryUsed
            })
            .ToListAsync();

        return Ok(successfulSubmissions);
    }



    [HttpPost("GetSubmissionsForProblem")]
    public async Task<IActionResult> GetSubmissionsForProblem(ProblemSubmissionsRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var problem = await _context.Problems.FindAsync(request.ProblemId);
        if (problem == null)
        {
            return NotFound($"Problem with ID {request.ProblemId} not found.");
        }

        var query = _context.UserSubmissions
            .Where(s => s.UserId == user.Id && s.ProblemID == request.ProblemId)
            .Include(s => s.Language);

        var totalCount = await query.CountAsync();

        var submissions = await query
            .OrderByDescending(s => s.SubmissionTime)
            .Select(s => new ProblemSubmissionDto
            {
                SubmissionId = s.SubmissionID,
                LanguageName = s.Language.Name,
                SubmissionTime = s.SubmissionTime,
                IsSuccessful = s.IsSuccessful,
                ExecutionTime = s.ExecutionTime,
                MemoryUsed = s.MemoryUsed
            })
            .ToListAsync();

        return Ok(submissions);
    }

    public class CountDto
    {
        public int Count { get; set; }
    }


    // Endpoint URL: POST /api/usersubmission/GetRecentSubmissions
    [HttpPost("GetRecentSubmissions")]
    public async Task<IActionResult> GetRecentSubmissions(CountDto countDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var recentSubmissions = await _context.UserSubmissions
            .Where(s => s.UserId == user.Id)
            .OrderByDescending(s => s.SubmissionTime)
            .Take(countDto.Count)
            .Select(s => new RecentSubmissionDto
            {
                SubmissionId = s.SubmissionID,
                ProblemId = s.ProblemID,
                ProblemTitle = s.Problem.Title,
                LanguageName = s.Language.Name,
                SubmissionTime = s.SubmissionTime,
                IsSuccessful = s.IsSuccessful
            })
            .ToListAsync();

        return Ok(recentSubmissions);
    }


    // Endpoint URL: POST /api/usersubmission/GetSubmissionDetails
    [HttpPost("GetSubmissionDetails")]
    public async Task<IActionResult> GetSubmissionDetails(RequestId request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var submission = await _context.UserSubmissions
            .Where(s => s.UserId == user.Id && s.SubmissionID == request.Id)
            .Select(s => new SubmissionDetailsDto
            {
                SubmissionId = s.SubmissionID,
                ProblemId = s.ProblemID,
                ProblemTitle = s.Problem.Title,
                LanguageName = s.Language.Name,
                SubmittedCode = s.SubmittedCode,
                SubmissionTime = s.SubmissionTime,
                IsSuccessful = s.IsSuccessful,
                ExecutionTime = s.ExecutionTime,
                MemoryUsed = s.MemoryUsed
            })
            .FirstOrDefaultAsync();

        if (submission == null)
        {
            return NotFound();
        }

        return Ok(submission);
    }


    [HttpGet("GetAbout")]
    public async Task<IActionResult> GetAbout()
    {
        try
        {
            var response = await _httpClient.GetAsync("/about");
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError getting about information from Judge0\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }


    [HttpGet("HealthCheck")]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            var response = await _httpClient.GetAsync("/about");
            return Ok($"Judge0 is reachable. Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nUnable to reach Judge0\n\n\n\n");
            return StatusCode(500, $"Unable to reach Judge0: {ex.Message}");
        }
    }

    public class TokenDto
    {
        public string Token { get; set; }
    }

    [HttpPost("GetSubmissionFromToken")]
    public async Task<IActionResult> GetSubmissionFromToken(TokenDto tokenDto)
    {
        _logger.LogInformation("\n\n\n\nGetting Submission Result for token: {tokenDto.Token}\n\n\n\n", tokenDto.Token);
        try
        {
            var response = await _httpClient.GetAsync($"submissions/{tokenDto.Token}?base64_encoded=true&fields=*");
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError getting submission result for token: {tokenDto.Token}\n\n\n\n", tokenDto.Token);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("GetLanguages")]
    public async Task<IActionResult> GetLanguages([FromQuery] int page = 1, [FromQuery] int perPage = 20)
    {
        _logger.LogInformation("\n\n\n\nGetting Languages\n\n\n\n");
        try
        {
            var response = await _httpClient.GetAsync($"languages?page={page}&per_page={perPage}");
            return await FormatResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nError getting languages\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("GetStatuses")]
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
        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("\n\n\n\nResponse Status Code: {response.StatusCode}\n\n\n\n", response.StatusCode);
        _logger.LogInformation("\n\n\n\nResponse Content: {content}\n\n\n\n", content);

        if (response.IsSuccessStatusCode)
        {
            return Content(content, "application/json");
        }

        return StatusCode((int)response.StatusCode, content);
    }
}