using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Encryption;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using NuGet.Protocol;
using System.Security.Claims;



namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly OurDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(OurDbContext context, ITokenService tokenService, ILogger<UsersController> logger, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _tokenService=tokenService;
            _logger = logger;
            _passwordHasher = passwordHasher;

        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("\n\n\n\nGetting list of Users.\n\n\n\n");
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/username/{username}
        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }


        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser(UserRegistrationDto userDto)
        {
            _logger.LogInformation($"\n\n\n\n\nATTEMPTING TO REGISTER USER {userDto.Username}\n\n\n\n\n");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model State is invalid.");
                return ValidationProblem(ModelState);
            }

            // new method to check for users!!

            var existingUser = await _context.Users
                .Where(u => u.Username == userDto.Username || u.EmailAddress == userDto.EmailAddress)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                string errorMessage = userDto.Username == existingUser.Username ? "Username already exists." : "Email address is already in use.";
                return Conflict(new { message = errorMessage });
            }

            // create a new User entity
            var newUser = new User
            {
                Username = userDto.Username,
                EmailAddress = userDto.EmailAddress
            };

            // hash the password
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, userDto.Password);


            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error occurred while creating the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A database error occured while creating the user. Please try again later.",
                    errorCode = "DB_ERROR",
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error occurred while creating the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unknown error occured. Please try again later.",
                    errorCode = "UNEXPECTED_ERROR",
                });
            }

            var token = _tokenService.GenerateToken(newUser);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };

            Response.Cookies.Append("auth_token", token, cookieOptions);


            // Return the JWT in the response
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, new { message = "User registered successfully" });
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(UserLoginDto userDto)
        {
            _logger.LogInformation("Attempting to log in user: {Username}", userDto.Username);

            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt for invalid username: {Username}", userDto.Username);
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                var passwordIsValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
                if (passwordIsValid == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Wrong password attempt for user: {Username}", user.Username);
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                if (passwordIsValid == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    _logger.LogInformation("User {Username} rehashing password", user.Username);
                    user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);
                }

                user.LastActiveDate = DateTime.UtcNow;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error occurred while updating user login information");
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        message = "A database error has occurred. Please try again later.",
                        errorCode = "DB_ERROR"
                    });
                }

                var token = _tokenService.GenerateToken(user);

                var cookieOptions = new CookieOptions
                {
                    Secure = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                };

                Response.Cookies.Append("auth_token", token, cookieOptions);

                _logger.LogInformation("User {Username} has been logged in", user.Username);
                return Ok(new { message = "Logged in successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error has occurred. Please try again later.",
                    errorCode = "UNEXPECTED_ERROR"
                });
            }
        }

        // POST: /api/Users/Logout
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token"); // need to delete the cookie to have an actual logout
            return Ok(new { message = "Logged out successfully" });
        }


  
        // GET: /api/Users/CheckSession
        [HttpGet("CheckSession")] 
        public ActionResult CheckSession() // should be useful when loading into pages to check for auth
        {
            var token = Request.Cookies["auth_token"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { isAuthenticated = false, message = "No authentication token found" });
            }

            var principal = _tokenService.ValidateToken(token);

            if (principal == null)
            {
                return Unauthorized(new { isAuthenticated = false, message = "Invalid authentication token" });
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = principal.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new { isAuthenticated = true, userId, username });
        }


    }
}
