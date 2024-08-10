using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;

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


    [HttpGet("GetProblemBasic")]
    public async Task<ActionResult<IEnumerable<Problem>>> GetProblemBasic()
    {
        return await _context.Problems.ToListAsync();
    }

    [HttpGet("GetProblemLanguageBasic")]
    public async Task<ActionResult<IEnumerable<ProblemLanguage>>> GetProblemLanguageBasic()
    {
        return await _context.ProblemLanguages.ToListAsync();
    }

    [HttpGet("GetLanguagesBasic")]
    public async Task<ActionResult<IEnumerable<Language>>> GetLanguagesBasic()
    {
        return await _context.Languages.ToListAsync();
    }



    [HttpPost("GetProblemsByCategory")]
    public async Task<IActionResult> GetProblemsByCategory(ProblemsByCategoryRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<ProblemCategory>(request.Category, true, out var category))
        {
            return BadRequest("Invalid category.");
        }

        var problems = await _context.Problems
            .Where(p => p.Category == category)
            .OrderBy(p => p.Difficulty)
            .ThenBy(p => p.Title)
            .Select(p => new FilteredProblemListItemDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                Difficulty = p.Difficulty.ToString(),
                Category = p.Category.ToString(),
                Points = p.Points,
                IsCompleted = p.UserSubmissions.Any(us => us.UserId == user.Id && us.IsSuccessful)
            })
            .ToListAsync();

        return Ok(problems);
    }




    [HttpPost("GetProblemsByDifficulty")]
    public async Task<IActionResult> GetProblemsByDifficulty(ProblemsByDifficultyRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficulty))
        {
            return BadRequest("Invalid difficulty level.");
        }

        var problems = await _context.Problems
            .Where(p => p.Difficulty == difficulty)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Title)
            .Select(p => new FilteredProblemListItemDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                Difficulty = p.Difficulty.ToString(),
                Category = p.Category.ToString(),
                Points = p.Points,
                IsCompleted = p.UserSubmissions.Any(us => us.UserId == user.Id && us.IsSuccessful)
            })
            .ToListAsync();

        return Ok(problems);
    }



    [HttpPost("AddProblem")]
    public async Task<IActionResult> AddProblem(AddProblemRequest request)
    {
        if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficulty))
        {
            return BadRequest("Invalid difficulty level.");
        }

        if (!Enum.TryParse<ProblemCategory>(request.Category, true, out var category))
        {
            return BadRequest("Invalid category.");
        }

        var problem = new Problem
        {
            Title = request.Title,
            Description = request.Description,
            Points = request.Points,
            Difficulty = difficulty,
            Category = category
        };

        _context.Problems.Add(problem);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New problem added: {ProblemID}", problem.ProblemID);

        return CreatedAtAction(nameof(GetProblemById), new { id = problem.ProblemID }, problem);
    }


    [HttpPost("AddLanguage")]
    public async Task<IActionResult> AddLanguage(AddLanguageRequest request)
    {
        var language = new Language
        {
            Name = request.Name,
            Judge0ID = request.Judge0ID
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync();

        _logger.LogInformation("\n\n\n\nNew language added: {LanguageID}\n\n\n\n", language.LanguageID);

        return CreatedAtAction(nameof(GetLanguageById), new { id = language.LanguageID }, language);
    }

    [HttpPost("AddProgramLanguage")]
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
            TestCode = request.TestCode,
            Problem = problem,
            Language = language,
        };

        _context.ProblemLanguages.Add(problemLanguage);

        // Explicitly set the navigation properties
        problemLanguage.Problem = problem;
        problemLanguage.Language = language;

        await _context.SaveChangesAsync();

        // Reload the ProblemLanguage to ensure all navigation properties are loaded
        await _context.Entry(problemLanguage)
            .Reference(pl => pl.Problem)
            .LoadAsync();
        await _context.Entry(problemLanguage)
            .Reference(pl => pl.Language)
            .LoadAsync();

        _logger.LogInformation("\n\n\n\nNew problem-language association added: {ProblemLanguageID}\n\n\n\n", problemLanguage.ProblemLanguageID);

        var dto = new ProblemLanguageDto
        {
            ProblemLanguageID = problemLanguage.ProblemLanguageID,
            ProblemID = problemLanguage.ProblemID,
            LanguageID = problemLanguage.LanguageID,
            FunctionSignature = problemLanguage.FunctionSignature,
            TestCode = problemLanguage.TestCode,
        };

        return CreatedAtAction(nameof(GetProblemLanguageById), new { id = problemLanguage.ProblemLanguageID }, dto);
    }



    // LETS YOU SEARCH BY EITHER/OR 
    [HttpPost("GetProblems")]
    public async Task<IActionResult> GetProblems(ProblemListRequest request)
    {

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }


        if (!string.IsNullOrEmpty(request.Category))
        {
            if (!Enum.TryParse<ProblemCategory>(request.Category, true, out var category))
            {
                return BadRequest("Invalid category.");
            }
            request.ParsedCategory = category;
        }

        if (!string.IsNullOrEmpty(request.Difficulty))
        {
            if (!Enum.TryParse<DifficultyLevel>(request.Difficulty, true, out var difficulty))
            {
                return BadRequest("Invalid difficulty level.");
            }
            request.ParsedDifficulty = difficulty;
        }

        var query = _context.Problems.AsQueryable();

        if (request.ParsedDifficulty.HasValue)
        {
            query = query.Where(p => p.Difficulty == request.ParsedDifficulty.Value);
        }

        if (request.ParsedCategory.HasValue)
        {
            query = query.Where(p => p.Category == request.ParsedCategory.Value);
        }

        var problems = await query
            .OrderBy(p => p.ProblemID)
            .Select(p => new ProblemListItemDto
            {
                ProblemID = p.ProblemID,
                Title = p.Title,
                IsCompleted = p.UserSubmissions.Any(us => us.UserId == user.Id && us.IsSuccessful),
                Difficulty = p.Difficulty.ToString(),
                Category = p.Category.ToString(),
                Points = p.Points
            })
            .ToListAsync();

        return Ok(problems);
    }





    [HttpPost("GetLanguageDetails")]
    public async Task<IActionResult> GetLanguageDetails()
    {

        var languages = await _context.Languages
            .OrderBy(l => l.Name)
            .Select(l => new LanguageListItemDto
            {
                LanguageID = l.LanguageID,
                Name = l.Name,
                Judge0ID = l.Judge0ID
            })
            .ToListAsync();

        return Ok(languages);
    }

    [HttpPost("GetProblemLanguages")]
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


        var problemLanguages = await query
            .OrderBy(pl => pl.ProblemLanguageID)
            .Select(pl => new ProblemLanguageListItemDto
            {
                ProblemLanguageID = pl.ProblemLanguageID,
                ProblemID = pl.ProblemID,
                LanguageID = pl.LanguageID,
                FunctionSignature = pl.FunctionSignature
            })
            .ToListAsync();

        return Ok(problemLanguages);
    }


    [HttpPost("GetProblemDetails")]
    public async Task<IActionResult> GetProblemDetails(RequestId request)
    {
        var problemDetails = await _context.Problems
            .Where(p => p.ProblemID == request.Id)
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
            return NotFound($"Problem with ID {request.Id} not found.");
        }

        return Ok(problemDetails);
    }



    [HttpPost("GetProblemById")]
    public async Task<IActionResult> GetProblemById(RequestId request)
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
            Description = problem.Description, // base64
            Points = problem.Points,
            Difficulty = problem.Difficulty,
            Category = problem.Category,
            AvailableLanguages = problem.ProblemLanguages.Select(pl => pl.Language.Name).ToList()
        };

        return Ok(problemDto);
    }

    [HttpPost("GetLanguageById")]
    public async Task<IActionResult> GetLanguageById(RequestId request)
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


    [HttpPost("GetProblemLanguageById")]
    public async Task<IActionResult> GetProblemLanguageById(RequestId request)
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
            FunctionSignature = problemLanguage.FunctionSignature, // base64
            TestCode = problemLanguage.TestCode // base64
        };

        return Ok(problemLanguageDto);
    }



    [HttpPut("UpdateProblem")]
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
            _logger.LogInformation("\n\n\n\nProblem with ID {ProblemID} has been updated.\n\n\n\n", request.ProblemID);
            return Ok($"Problem with ID {request.ProblemID} has been successfully updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nAn error occurred while updating Problem with ID {ProblemID}\n\n\n\n", request.ProblemID);
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    [HttpPut("UpdateLanguage")]
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
            _logger.LogInformation("\n\n\n\nLanguage with ID {LanguageID} has been updated.\n\n\n\n", request.LanguageID);
            return Ok($"Language with ID {request.LanguageID} has been successfully updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nAn error occurred while updating Language with ID {LanguageID}\n\n\n\n", request.LanguageID);
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    [HttpPut("UpdateProblemLanguage")]
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
            _logger.LogInformation("\n\n\n\nProblemLanguage with ID {ProblemLanguageID} has been updated.\n\n\n\n", request.ProblemLanguageID);
            return Ok($"ProblemLanguage with ID {request.ProblemLanguageID} has been successfully updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nAn error occurred while updating ProblemLanguage with ID {ProblemLanguageID}\n\n\n\n", request.ProblemLanguageID);
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }




    [HttpPost("DeleteProblem")]
    public async Task<IActionResult> DeleteProblem(RequestId request)
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
            _logger.LogInformation("\n\n\n\nProblem with ID {Id} and its associated ProblemLanguages have been deleted.\n\n\n\n", request.Id);
            return Ok($"Problem with ID {request.Id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nAn error occurred while deleting Problem with ID {Id}\n\n\n\n", request.Id);
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }

    [HttpPost("DeleteLanguage")]
    public async Task<IActionResult> DeleteLanguage(RequestId request)
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
            _logger.LogInformation("\n\n\n\nLanguage with ID {request.Id} has been deleted.\n\n\n\n", request.Id);
            return Ok($"Language with ID {request.Id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nAn error occurred while deleting Language with ID {Id}\n\n\n\n", request.Id);
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }



    [HttpPost("DeleteProblemLanguage")]
    public async Task<IActionResult> DeleteProblemLanguage(RequestId request)
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
            _logger.LogInformation("\n\n\n\nProblemLanguage with ID {Id} has been deleted.\n\n\n\n", request.Id);
            return Ok($"ProblemLanguage with ID {request.Id} has been successfully deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "\n\n\n\nAn error occurred while deleting ProblemLanguage with ID {Id}\n\n\n\n", request.Id);
            return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        }
    }
}
