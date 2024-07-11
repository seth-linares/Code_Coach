using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Authorization;



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

        private async Task<bool> SendLinkEmailAsync(User user, string subject, string linkText, string linkPath, string tokenType)
        {
            var token = await _userManager.GenerateUserTokenAsync(user, "Default", tokenType);
            var link = $"{_configuration["FrontendUrl"]}/{linkPath}?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

            return await _emailService.SendEmailAsync(
                user.Email,
                subject,
                $"{linkText}: {link}",
                $"{linkText}: <a href='{link}'>{linkText}</a>"
            );
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("\n\n\n\nGetting list of Users.\n\n\n\n");
            return await _context.Users.ToListAsync();
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto userDto)
        {
            _logger.LogInformation("Attempting to register new user");
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                // Check if user already exists (by email or username)
                var existingUserEmail = await _userManager.FindByEmailAsync(userDto.EmailAddress);
                var existingUserName = await _userManager.FindByNameAsync(userDto.Username);
                if (existingUserEmail != null)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return ValidationProblem(ModelState);
                }
                if (existingUserName != null)
                {
                    ModelState.AddModelError("UserName", "This username is already taken.");
                    return ValidationProblem(ModelState);
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
                    return ValidationProblem(ModelState);
                }

                _logger.LogInformation($"User created: {user.UserName} (ID: {user.Id})");

                var emailSent = await SendLinkEmailAsync(
                    user,
                    "Confirm your email",
                    "Please confirm your account by clicking this link",
                    "confirm-email",
                    "EmailConfirmation"
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send confirmation email to user {user.Id}");
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, "User registered but failed to send confirmation email. Please try again.");
                }

                _logger.LogInformation($"Confirmation email sent to: {user.UserName} (ID: {user.Id})");

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


        //// POST: api/Users/Enable2FA
        //[HttpPost("Enable2FA")]
        //public async 

        

        // POST: api/Users/ConfirmEmail
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {

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





        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
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

                var result = await _signInManager.PasswordSignInAsync(user, userDto.Password, isPersistent: userDto.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return await CompleteLoginAsync(user);
                }

                if (result.RequiresTwoFactor)
                {
                    return await InitiateTwoFactorAuthenticationAsync(user);
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

        private async Task<IActionResult> CompleteLoginAsync(User user)
        {
            user.LastActiveDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User {user.UserName} logged in successfully");
            return Ok(new { message = "Logged in successfully" });
        }

        private async Task<IActionResult> InitiateTwoFactorAuthenticationAsync(User user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailService.SendEmailAsync(user.Email, "Your 2FA Code", $"Your login code is: {token}");

            _logger.LogInformation($"2FA initiated for user: {user.UserName}");
            return Ok(new { requiresTwoFactor = true, message = "2FA code sent to your email." });
        }

        [HttpPost("VerifyTwoFactorCode")]
        public async Task<IActionResult> VerifyTwoFactorCode(TwoFactorVerificationDto twoFactorDto)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest("Invalid request");
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", twoFactorDto.Code, twoFactorDto.RememberMe, twoFactorDto.RememberBrowser);
            if (result.Succeeded)
            {
                return await CompleteLoginAsync(user);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning($"User account locked out during 2FA: {user.UserName}");
                return StatusCode(StatusCodes.Status423Locked, new { message = "Account is locked. Please try again later." });
            }
            else
            {
                _logger.LogWarning($"Invalid 2FA code attempt for user: {user.UserName}");
                return BadRequest("Invalid verification code");
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

        // POST: /api/Users/ChangePassword
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            _logger.LogInformation($"\n\n\n\nAttempting to change password");

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user,
                changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return ValidationProblem(ModelState);
            }

            // Optional: Sign in the user again to update the authentication cookie
            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation($"\n\n\n\nUser {user.UserName} successfully changed their password.\n\n\n\n");

            return Ok("Your password has been changed successfully.");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            _logger.LogInformation($"\n\n\n\nATTEMPTING TO HANDLE FORGOT PASSWORD WITH EMAIL: {forgotPasswordDto.Email}\n\n\n\n");
            
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
                

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                _logger.LogInformation($"\n\n\n\nFAKE USER EMAIL: {forgotPasswordDto.Email}\n\n\n\n");
                // Don't reveal that the user does not exist or is not confirmed
                return Ok("If your email is registered and confirmed, you will receive a password reset link shortly.");
            }

            var emailSent = await SendLinkEmailAsync(
                user,
                "Reset your password",
                "Please reset your password by clicking here",
                "reset-password",
                "ResetPassword"
            );

            if (!emailSent)
            {
                _logger.LogWarning($"\n\n\n\nFailed to send password reset email to user {user.Id}\n\n\n\n");
                return StatusCode(500, "Failed to send password reset email. Please try again later.");
            }

            return Ok("If your email is registered and confirmed, you will receive a password reset link shortly.");
        }

        
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation($"\n\n\n\nATTEMPTING TO RESET PASSWORD\n\n\n\n");
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
                

            var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
            if (user == null)
            {
                _logger.LogInformation($"\n\n\n\nUSER NOT FOUND IN RESET\n\n\n\n");
                // Don't reveal that the user does not exist
                return Ok("If your account exists, your password has been reset.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation($"\n\n\n\nPassword reset successfully for user {user.Id}\n\n\n\n");
                return Ok("Your password has been reset successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogInformation($"\n\n\n\nERRORS CAME UP WHEN RESETTING PASSWORD\n\n\n\n");
            return ValidationProblem(ModelState);
        }


        // POST: api/Users/DeleteAccount
        [Authorize]
        [HttpPost("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(DeleteAccountDto deleteAccountDto)
        {
            _logger.LogInformation($"\n\n\n\nAttemping to delete account\n\n\n\n");
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("User", "User not found.");
                return ValidationProblem(ModelState);
            }

            var result = await _userManager.CheckPasswordAsync(user, deleteAccountDto.Password);
            if (!result)
            {
                ModelState.AddModelError("Password", "Incorrect password");
                return ValidationProblem(ModelState);
            }

            try
            {
                // Delete the user - this will trigger cascade deletes for related entities
                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    _logger.LogError($"\n\n\n\nFailed to delete user: {user.UserName} (ID: {user.Id})\n\n\n\n");
                    // You can include error details from deleteResult.Errors if they provide useful context
                    return StatusCode(500, "Failed to delete user. Please try again later.");
                }

                // Log the account deletion
                _logger.LogInformation($"User account deleted: {user.UserName} (ID: {user.Id})");

                // Sign out the user
                await _signInManager.SignOutAsync();

                return Ok("Your account has been successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user account: {user.UserName} (ID: {user.Id})");
                return StatusCode(500, "An error occurred while deleting your account. Please try again later.");
            }
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


        


        



    }
}
