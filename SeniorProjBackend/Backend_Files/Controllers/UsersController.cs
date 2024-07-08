using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Encryption;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using NuGet.Protocol;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;



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
            if (id != user.UserId)
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
            return _context.Users.Any(e => e.UserId == id);
        }


        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser(UserRegistrationDto userDto)
        {
            _logger.LogInformation($"\n\n\n\n\nATTEMPTING TO REGISTER USER {userDto.Username}\n\n\n\n\n");

            if (!ModelState.IsValid)
            {
                _logger.LogError("\n\n\n\nModel State is invalid.\n\n\n\n");
                return ValidationProblem(ModelState);
            }

            // new method to check for users!!

            var existingUser = await _context.Users
                .Where(u => u.Username == userDto.Username || u.EmailAddress == userDto.EmailAddress)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                bool sameUsername = userDto.Username == existingUser.Username;

                ModelState.AddModelError(
                    sameUsername ? "Username" : "EmailAddress",
                    sameUsername ? "Username already exists." : "Email address is already in use."
                );


                return ValidationProblem(ModelState);
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
                _logger.LogError($"\n\n\n\n{ex}, Database error occurred while creating the user\n\n\n\n");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A database error occured while creating the user. Please try again later.",
                    errorCode = "DB_ERROR",
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"\n\n\n\n{ex}, Unexpected error occurred while creating the user\n\n\n\n");
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
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, new { message = "User registered successfully" });
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(UserLoginDto userDto)
        {
            _logger.LogInformation($"\n\n\n\nAttempting to log in user: {userDto.Username}\n\n\n\n");

            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
                if (user == null)
                {
                    _logger.LogWarning($"\n\n\n\nFailed login attempt for invalid username: {userDto.Username}\n\n\n\n");
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                var passwordIsValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
                if (passwordIsValid == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning($"\n\n\n\nWrong password attempt for user: {user.Username}\n\n\n\n");
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                if (passwordIsValid == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    _logger.LogInformation($"\n\n\n\nUser {user.Username} rehashing password\n\n\n\n");
                    user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);
                }

                user.LastActiveDate = DateTime.UtcNow;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"\n\n\n\n{ex}, Database error occurred while updating user login information\n\n\n\n");
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

                _logger.LogDebug($"\n\n\n\nUser {user.Username} has been logged in\n\n\n\n");
                return Ok(new { message = "Logged in successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"\n\n\n\n{ex}, Unexpected error during login\n\n\n\n");
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
        public ActionResult CheckSession()
        {
            var authCookie = Request.Cookies["auth_token"];
            _logger.LogInformation($"\n\n\n\nauthCookie: {authCookie}\n");

            foreach(string header in Request.Headers.Keys)
            {
                _logger.LogInformation($"\n\n\n\nHeader: {header}\n\n\n\n");
            }

            var diagnosticInfo = new
            {
                HasAuthCookie = !string.IsNullOrEmpty(authCookie),
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                return Ok(new { isAuthenticated = true, userId, username, diagnosticInfo });
            }
            else
            {
                return Ok(new { isAuthenticated = false, message = "No valid authentication token found", diagnosticInfo });
            }
        }


    }
}
