using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using System.Threading.Tasks;
using SeniorProjBackend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ProblemManagementController : ControllerBase
{
    private readonly OurDbContext _context;
    private readonly ILogger<ProblemManagementController> _logger;
    private readonly UserManager<User> _userManager;

    public ProblemManagementController(OurDbContext context, ILogger<ProblemManagementController> logger, UserManager<User> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpPost("problem")]
    public async Task<IActionResult> AddProblem(AddProblemRequest request)
    {
        var problem = new Problem
        {
            Title = request.Title,
            Description = request.Description,
            Points = request.Points,
            Difficulty = request.Difficulty,
            Category = request.Category
        };

        _context.Problems.Add(problem);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"\n\n\n\nNew problem added: {problem.ProblemID}\n\n\n\n");

        return CreatedAtAction(nameof(GetProblem), new { id = problem.ProblemID }, problem);
    }

    [HttpPost("language")]
    public async Task<IActionResult> AddLanguage(AddLanguageRequest request)
    {
        var language = new Language
        {
            Name = request.Name,
            Judge0ID = request.Judge0ID
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"\n\n\n\nNew language added: {language.LanguageID}\n\n\n\n");

        return CreatedAtAction(nameof(GetLanguage), new { id = language.LanguageID }, language);
    }

    [HttpPost("problem-language")]
    public async Task<IActionResult> AddProblemLanguage(AddProblemLanguageRequest request)
    {
        var problem = await _context.Problems.FindAsync(request.ProblemID);
        var language = await _context.Languages.FindAsync(request.LanguageID);

        if (problem == null || language == null)
        {
            return NotFound("Problem or Language not found");
        }

        var problemLanguage = new ProblemLanguage
        {
            ProblemID = request.ProblemID,
            LanguageID = request.LanguageID,
            FunctionSignature = request.FunctionSignature,
            TestCode = request.TestCode
        };

        _context.ProblemLanguages.Add(problemLanguage);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"\n\n\n\nNew problem-language association added: {problemLanguage.ProblemLanguageID}\n\n\n\n");

        return CreatedAtAction(nameof(GetProblemLanguage), new { id = problemLanguage.ProblemLanguageID }, problemLanguage);
    }

    [HttpGet("problems")]
    public async Task<IActionResult> GetProblems(ProblemListRequest request)
    {
        var query = _context.Problems.AsQueryable();

        if (request.Difficulty.HasValue)
        {
            query = query.Where(p => p.Difficulty == request.Difficulty.Value);
        }

        if (request.Category.HasValue)
        {
            query = query.Where(p => p.Category == request.Category.Value);
        }

        var totalCount = await query.CountAsync();

        var problems = await query
            .OrderBy(p => p.ProblemID)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProblemListItemDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                Difficulty = p.Difficulty,
                Category = p.Category,
                Points = p.Points
            })
            .ToListAsync();

        var response = new PaginatedResponse<ProblemListItemDto>
        {
            Items = problems,
            TotalCount = totalCount,
            PageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Ok(response);
    }

    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages(PaginationRequest request)
    {
        var totalCount = await _context.Languages.CountAsync();

        var languages = await _context.Languages
            .OrderBy(l => l.LanguageID)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LanguageListItemDto
            {
                LanguageID = l.LanguageID,
                Name = l.Name,
                Judge0ID = l.Judge0ID
            })
            .ToListAsync();

        var response = new PaginatedResponse<LanguageListItemDto>
        {
            Items = languages,
            TotalCount = totalCount,
            PageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Ok(response);
    }

    [HttpGet("problem-languages")]
    public async Task<IActionResult> GetProblemLanguages(ProblemLanguageListRequest request)
    {
        var query = _context.ProblemLanguages.AsQueryable();

        if (request.ProblemID.HasValue)
        {
            query = query.Where(pl => pl.ProblemID == request.ProblemID.Value);
        }

        if (request.LanguageID.HasValue)
        {
            query = query.Where(pl => pl.LanguageID == request.LanguageID.Value);
        }

        var totalCount = await query.CountAsync();

        var problemLanguages = await query
            .OrderBy(pl => pl.ProblemLanguageID)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pl => new ProblemLanguageListItemDto
            {
                ProblemLanguageID = pl.ProblemLanguageID,
                ProblemID = pl.ProblemID,
                LanguageID = pl.LanguageID,
                FunctionSignature = pl.FunctionSignature
            })
            .ToListAsync();

        var response = new PaginatedResponse<ProblemLanguageListItemDto>
        {
            Items = problemLanguages,
            TotalCount = totalCount,
            PageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Ok(response);
    }


    [HttpPost("problems-by-category")]
    public async Task<IActionResult> GetProblemsByCategory(ProblemsByCategoryRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var problems = await _context.Problems
            .Where(p => p.Category == request.Category)
            .OrderBy(p => p.Difficulty)
            .ThenBy(p => p.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new FilteredProblemListItemDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                Difficulty = p.Difficulty,
                Points = p.Points,
                IsCompleted = p.UserSubmissions.Any(us => us.UserId == user.Id && us.IsSuccessful)
            })
            .ToListAsync();

        var totalCount = await _context.Problems
            .Where(p => p.Category == request.Category)
            .CountAsync();

        var response = new PaginatedResponse<FilteredProblemListItemDto>
        {
            Items = problems,
            TotalCount = totalCount,
            PageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Ok(response);
    }

    [HttpPost("problems-by-difficulty")]
    public async Task<IActionResult> GetProblemsByDifficulty(ProblemsByDifficultyRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var problems = await _context.Problems
            .Where(p => p.Difficulty == request.Difficulty)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new FilteredProblemListItemDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                Category = p.Category,
                Points = p.Points,
                IsCompleted = p.UserSubmissions.Any(us => us.UserId == user.Id && us.IsSuccessful)
            })
            .ToListAsync();

        var totalCount = await _context.Problems
            .Where(p => p.Difficulty == request.Difficulty)
            .CountAsync();

        var response = new PaginatedResponse<FilteredProblemListItemDto>
        {
            Items = problems,
            TotalCount = totalCount,
            PageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Ok(response);
    }

    [HttpPost("problem-details")]
    public async Task<IActionResult> GetProblemDetails(ProblemDetailsRequest request)
    {
        var problemDetails = await _context.Problems
            .Where(p => p.ProblemID == request.ProblemId)
            .Select(p => new ProblemDetailsDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                Description = p.Description,
                Difficulty = p.Difficulty,
                Category = p.Category,
                Points = p.Points,
                LanguageDetails = p.ProblemLanguages.Select(pl => new ProblemLanguageDetailsDto
                {
                    LanguageID = pl.LanguageID,
                    LanguageName = pl.Language.Name,
                    Judge0LanguageId = pl.Language.Judge0ID,
                    FunctionSignature = pl.FunctionSignature
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (problemDetails == null)
        {
            return NotFound($"Problem with ID {request.ProblemId} not found.");
        }

        return Ok(problemDetails);
    }



    [HttpPost("get-problem")]
    public async Task<IActionResult> GetProblem(GetByIdRequest request)
    {
        var problem = await _context.Problems
            .Include(p => p.ProblemLanguages)
            .ThenInclude(pl => pl.Language)
            .FirstOrDefaultAsync(p => p.ProblemID == request.Id);

        if (problem == null)
        {
            return NotFound($"Problem with ID {request.Id} not found.");
        }

        var problemDto = new ProblemDetailDto
        {
            ProblemID = problem.ProblemID,
            Title = problem.Title,
            Description = problem.Description,
            Points = problem.Points,
            Difficulty = problem.Difficulty,
            Category = problem.Category,
            AvailableLanguages = problem.ProblemLanguages.Select(pl => pl.Language.Name).ToList()
        };

        return Ok(problemDto);
    }

    [HttpPost("get-language")]
    public async Task<IActionResult> GetLanguage(GetByIdRequest request)
    {
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.LanguageID == request.Id);

        if (language == null)
        {
            return NotFound($"Language with ID {request.Id} not found.");
        }

        var languageDto = new LanguageDetailDto
        {
            LanguageID = language.LanguageID,
            Name = language.Name,
            Judge0ID = language.Judge0ID
        };

        return Ok(languageDto);
    }

    public class GetByIdRequest
    {
        public int Id { get; set; }
    }

    [HttpPost("get-problem-language")]
    public async Task<IActionResult> GetProblemLanguage(GetByIdRequest request)
    {
        var problemLanguage = await _context.ProblemLanguages
            .Include(pl => pl.Problem)
            .Include(pl => pl.Language)
            .FirstOrDefaultAsync(pl => pl.ProblemLanguageID == request.Id);

        if (problemLanguage == null)
        {
            return NotFound($"ProblemLanguage with ID {request.Id} not found.");
        }

        var problemLanguageDto = new ProblemLanguageDetailDto
        {
            ProblemLanguageID = problemLanguage.ProblemLanguageID,
            ProblemID = problemLanguage.ProblemID,
            ProblemTitle = problemLanguage.Problem.Title,
            LanguageID = problemLanguage.LanguageID,
            LanguageName = problemLanguage.Language.Name,
            FunctionSignature = problemLanguage.FunctionSignature,
            TestCode = problemLanguage.TestCode
        };

        return Ok(problemLanguageDto);
    }



    [HttpPut("update-problem")]
    public async Task<IActionResult> UpdateProblem(UpdateProblemRequest request)
    {
        var problem = await _context.Problems.FindAsync(request.ProblemID);

        if (problem == null)
        {
            return NotFound($"Problem with ID {request.ProblemID} not found.");
        }

        problem.Title = request.Title;
        problem.Description = request.Description;
        problem.Points = request.Points;
        problem.Difficulty = request.Difficulty;
        problem.Category = request.Category;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\n\n\n\nProblem with ID {request.ProblemID} has been updated.\n\n\n\n");
            return Ok($"Problem with ID {request.ProblemID} has been successfully updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n\n\n\nException: {ex}\nAn error occurred while updating Problem with ID {request.ProblemID}\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    [HttpPut("update-language")]
    public async Task<IActionResult> UpdateLanguage(UpdateLanguageRequest request)
    {
        var language = await _context.Languages.FindAsync(request.LanguageID);

        if (language == null)
        {
            return NotFound($"Language with ID {request.LanguageID} not found.");
        }

        language.Name = request.Name;
        language.Judge0ID = request.Judge0ID;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\n\n\n\nLanguage with ID {request.LanguageID} has been updated.\n\n\n\n");
            return Ok($"Language with ID {request.LanguageID} has been successfully updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n\n\n\nException: {ex}\nAn error occurred while updating Language with ID {request.LanguageID}\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    [HttpPut("update-problem-language")]
    public async Task<IActionResult> UpdateProblemLanguage(UpdateProblemLanguageRequest request)
    {
        var problemLanguage = await _context.ProblemLanguages.FindAsync(request.ProblemLanguageID);

        if (problemLanguage == null)
        {
            return NotFound($"ProblemLanguage with ID {request.ProblemLanguageID} not found.");
        }

        problemLanguage.FunctionSignature = request.FunctionSignature;
        problemLanguage.TestCode = request.TestCode;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\n\n\n\nProblemLanguage with ID {request.ProblemLanguageID} has been updated.\n\n\n\n");
            return Ok($"ProblemLanguage with ID {request.ProblemLanguageID} has been successfully updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"\n\n\n\nAn error occurred while updating ProblemLanguage with ID {request.ProblemLanguageID}\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }




    public class DeleteRequest
    {
        public int Id { get; set; }
    }

    [HttpPost("delete-problem")]
    public async Task<IActionResult> DeleteProblem(DeleteRequest request)
    {
        var problem = await _context.Problems
            .Include(p => p.ProblemLanguages)
            .FirstOrDefaultAsync(p => p.ProblemID == request.Id);

        if (problem == null)
        {
            return NotFound($"Problem with ID {request.Id} not found.");
        }

        // Remove associated ProblemLanguages
        _context.ProblemLanguages.RemoveRange(problem.ProblemLanguages);

        // Remove the Problem
        _context.Problems.Remove(problem);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\n\n\n\nProblem with ID {request.Id} and its associated ProblemLanguages have been deleted.\n\n\n\n");
            return Ok($"Problem with ID {request.Id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n\n\n\nException: {ex}\nAn error occurred while deleting Problem with ID {request.Id}\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    [HttpPost("delete-language")]
    public async Task<IActionResult> DeleteLanguage(DeleteRequest request)
    {
        var language = await _context.Languages
            .Include(l => l.ProblemLanguages)
            .FirstOrDefaultAsync(l => l.LanguageID == request.Id);

        if (language == null)
        {
            return NotFound($"Language with ID {request.Id} not found.");
        }

        // Check if there are any ProblemLanguages associated with this Language
        if (language.ProblemLanguages.Any())
        {
            return BadRequest($"Cannot delete Language with ID {request.Id} as it is associated with one or more problems. Remove these associations first.");
        }

        _context.Languages.Remove(language);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\n\n\n\nLanguage with ID {request.Id} has been deleted.\n\n\n\n");
            return Ok($"Language with ID {request.Id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n\n\n\nException: {ex}An error occurred while deleting Language with ID {request.Id}\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    

    [HttpPost("delete-problem-language")]
    public async Task<IActionResult> DeleteProblemLanguage(DeleteRequest request)
    {
        var problemLanguage = await _context.ProblemLanguages
            .FirstOrDefaultAsync(pl => pl.ProblemLanguageID == request.Id);

        if (problemLanguage == null)
        {
            return NotFound($"ProblemLanguage with ID {request.Id} not found.");
        }

        _context.ProblemLanguages.Remove(problemLanguage);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"\n\n\n\nProblemLanguage with ID {request.Id} has been deleted.\n\n\n\n");
            return Ok($"ProblemLanguage with ID {request.Id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"\n\n\n\nException: {ex}\nAn error occurred while deleting ProblemLanguage with ID {request.Id}\n\n\n\n");
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }
}
