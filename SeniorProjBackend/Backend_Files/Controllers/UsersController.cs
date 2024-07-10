using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Net;



namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UsersController(OurDbContext context, ILogger<UsersController> logger, UserManager<User> userManager,
            SignInManager<User> signInManager, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpGet("testEmail")]
        public IActionResult TestEmail()
        {
            _logger.LogInformation("TestEmail endpoint called");
            var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
            _logger.LogInformation("EmailService retrieved from DI container");
            return Ok("Email service test completed");
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied. You do not have permission to access this resource." });
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
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

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
            if (id != user.Id)
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
            return _context.Users.Any(e => e.Id == id);
        }

        // POST: api/Users/ConfirmEmail
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{dto.UserId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Failed to confirm email for user {dto.UserId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return BadRequest("Error confirming your email. Please try again or contact support.");
            }

            _logger.LogInformation($"Email confirmed for user {dto.UserId}");
            return Ok(new { message = "Email confirmed successfully. You can now log in to your account." });
        }


        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto userDto)
        {
            _logger.LogInformation("\n\n\n\nTRYING TO REGISTER\n\n\n\n");
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if user already exists (by email or username)
                var existingUserEmail = await _userManager.FindByEmailAsync(userDto.EmailAddress);
                var existingUserName = await _userManager.FindByNameAsync(userDto.Username);
                if (existingUserEmail != null)
                {
                    return Conflict("A user with this email already exists.");
                }
                if (existingUserName != null)
                {
                    return Conflict("This username is already taken.");
                }

                var user = new User
                {
                    UserName = userDto.Username,
                    Email = userDto.EmailAddress,
                    RegistrationDate = DateTime.UtcNow,
                    LastActiveDate = DateTime.UtcNow
                };

                

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("\n\n\n\nUSER CREATED\n\n\n\n");

                //// Add user to a default role if you're using roles
                //await _userManager.AddToRoleAsync(user, "User");

                var emailSent = await SendConfirmationEmailAsync(user);

                _logger.LogInformation($"\n\n\n\nTRYING TO SEND EMAIL, emailSent: {emailSent}\n\n\n\n");

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send confirmation email to user {user.Id}");
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, "User registered but failed to send confirmation email. Please try again.");
                }

                _logger.LogInformation($"New user registered: {user.UserName} (ID: {user.Id})");

                return Ok(new
                {
                    message = "Registration successful. Please check your email to confirm your account before logging in.",
                    userId = user.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private async Task<bool> SendConfirmationEmailAsync(User user)
        {
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?userId={user.Id}&token={WebUtility.UrlEncode(confirmationToken)}";

            return await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                "Please confirm your account by clicking this link: ",
                $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>"
            );
        }



        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(UserLoginDto userDto)
        {
            _logger.LogInformation($"Attempting to log in user: {userDto.Username}");

            try
            {
                var user = await _userManager.FindByNameAsync(userDto.Username);
                if (user == null)
                {
                    _logger.LogWarning($"Failed login attempt for invalid username: {userDto.Username}");
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogWarning($"Login attempt for unconfirmed email: {user.Email}");
                    return Unauthorized(new { message = "Please confirm your email before logging in." });
                }

                var result = await _signInManager.PasswordSignInAsync(user, userDto.Password, isPersistent: true, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastActiveDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation($"User {user.UserName} logged in successfully");
                    return Ok(new { message = "Logged in successfully" });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User account locked out: {user.UserName}");
                    return StatusCode(StatusCodes.Status423Locked, new { message = "Account is locked. Please try again later." });
                }

                _logger.LogWarning($"Failed login attempt for user: {user.UserName}");
                return Unauthorized(new { message = "Invalid username or password." });
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
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"\n\n\nLogging user out!!!\n\n\n");
            return Ok(new { message = "Logged out successfully" });
        }


        // GET: /api/Users/CheckSession
        [HttpGet("CheckSession")]
        public async Task<IActionResult> CheckSession()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Only update LastActiveDate if it's been more than 5 minutes
                    if (DateTime.UtcNow - user.LastActiveDate > TimeSpan.FromMinutes(5))
                    {
                        user.LastActiveDate = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    return Ok(new
                    {
                        isAuthenticated = true,
                        userId = user.Id,
                        username = user.UserName,
                        email = user.Email,
                        lastActiveDate = user.LastActiveDate,
                    });
                }
            }

            return Ok(new { isAuthenticated = false });
        }



    }
}
