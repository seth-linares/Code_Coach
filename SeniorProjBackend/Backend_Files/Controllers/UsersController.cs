using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Middleware;
using System.ComponentModel.DataAnnotations;
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

        public UsersController(OurDbContext context, ILogger<UsersController> logger, UserManager<User> userManager,
            SignInManager<User> signInManager, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        private async Task<bool> SendLinkEmailAsync(User user, string subject, string linkText, string linkPath, string tokenType)
        {
            // Explicitly set the base URL
            var baseUrl = "https://www.codecoachapp.com"; // Prod URL

            var token = await _userManager.GenerateUserTokenAsync(user, "Default", tokenType);
            var link = $"{baseUrl}/{linkPath}?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

            // Log a redacted version of the link
            var redactedLink = link.Replace(token, "[REDACTED]");
            _logger.LogInformation("\n\n\n\nbase url: {baseUrl}\nRedacted Link: {redactedLink}\n\n\n\n", baseUrl, redactedLink);

            return await _emailService.TrySendEmailAsync(
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
            _logger.LogInformation("\n\n\n\nAttempting to register new user\n\n\n\n");
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
                    _logger.LogError("\n\n\n\nEMAIL ALREADY EXISTS {EmailAddress}\n\n\n\n", userDto.EmailAddress);
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return ValidationProblem(ModelState);
                }
                if (existingUserName != null)
                {
                    _logger.LogError("\n\n\n\nUSERNAME ALREADY EXISTS {userDto.Username}\n\n\n\n", userDto.Username);
                    ModelState.AddModelError("UserName", "This username is already taken.");
                    return ValidationProblem(ModelState);
                }

                var user = new User
                {
                    UserName = userDto.Username,
                    Email = userDto.EmailAddress,
                    RegistrationDate = DateTimeOffset.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("\n\n\n\nERRORS OCCURED WHILE REGISTERING\n");
                        _logger.LogError("Error: {error}\n\n\n\n", error);
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return ValidationProblem(ModelState);
                }

                _logger.LogInformation("\n\n\n\nUser created: {user.UserName} (ID: {user.Id})\n\n\n\n", user.UserName, user.Id);

                var emailSent = await SendLinkEmailAsync(
                    user,
                    "Confirm your email",
                    "Please confirm your account by clicking this link",
                    "confirm-email",
                    "EmailConfirmation"
                );

                if (!emailSent)
                {
                    _logger.LogWarning("\n\n\n\nFailed to send confirmation email to user {user.Id}\n\n\n\n", user.Id);
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, "User registered but failed to send confirmation email. Please try again.");
                }

                _logger.LogInformation("\n\n\n\nConfirmation email sent to: {user.UserName} (ID: {user.Id})\n\n\n\n", user.UserName, user.Id);

                return Ok(new
                {
                    message = "Registration successful. Please check your email to confirm your account before logging in.",
                    userId = user.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "\n\n\n\nAn error occurred during user registration\n\n\n\n");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        // POST: api/Users/Enable2FA
        [Authorize]
        [HttpPost("Enable2FA")]
        public async Task<IActionResult> Enable2FA()
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO ENABLE 2FA\n\n\n\n");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("2FA is already enabled for this user");
            }

            // Generate a verification token
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send the verification email
            var emailSent = await _emailService.TrySendEmailAsync(
                user.Email,
                "Verify your email for 2FA",
                $"Your verification code is: {token}");

            if (!emailSent)
            {
                return StatusCode(500, "Failed to send verification email");
            }

            _logger.LogInformation("\n\n\n\n2FA verification email sent to user: {user.UserName}\n\n\n\n", user.UserName);
            return Ok(new { message = "Verification code sent to your email. Please verify to enable 2FA." });
        }

        public class VerificationDto
        {
            public string Code { get; set; }
        }

        // POST: api/Users/VerifyAnd2FA
        [Authorize]
        [HttpPost("VerifyAnd2FA")]
        public async Task<IActionResult> VerifyAnd2FA(VerificationDto verificationDto)
        {
            _logger.LogInformation("\n\n\n\nVerification code: {verificationDto.Code}\n\n\n\n", verificationDto.Code);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", verificationDto.Code);
            if (!isValid)
            {
                _logger.LogInformation("\n\n\n\nBAD VERIFICATION CODE\n\n\n\n");
                return BadRequest(new { message = "Invalid verification code" });
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (result.Succeeded)
            {
                _logger.LogInformation("\n\n\n\n2FA enabled for user: {user.UserName}\n\n\n\n", user.UserName);
                return Ok(new { message = "2FA has been enabled successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to enable 2FA", errors = result.Errors.Select(e => e.Description) });
            }
        }

        [Authorize]
        [HttpPost("Disable2FA")]
        public async Task<IActionResult> Disable2FA()
        {
            _logger.LogInformation("\n\n\n\nDisable2FA\n\n\n\n");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("\n\n\n\n2FA disabled for user: {user.UserName}\n\n\n\n", user.UserName);
                return Ok(new { message = "2FA has been disabled successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to disable 2FA", errors = result.Errors.Select(e => e.Description) });
            }
        }

        [Authorize]
        [HttpGet("2FAStatus")]
        public async Task<IActionResult> Get2FAStatus()
        {
            _logger.LogInformation("\n\n\n\nGetting 2FA Status\n\n\n\n");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            _logger.LogInformation("\n\n\n\nis2faEnabled: {is2faEnabled}\n\n\n\n", is2faEnabled);
            return Ok(new { is2faEnabled });
        }



        // POST: api/Users/ConfirmEmail
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {
            _logger.LogInformation("\n\n\n\nAttempting to confirm email for user ID: {dto.UserId}\n\n\n\n", dto.UserId);

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("\n\n\n\nUnable to load user with ID '{dto.UserId}'.\n\n\n\n", dto.UserId);
                return NotFound($"Unable to load user with ID '{dto.UserId}'.");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("\n\n\n\nEmail already confirmed for user {dto.UserId}\n\n\n\n", dto.UserId);
                return Ok(new { message = "Email already confirmed. You can log in to your account." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!result.Succeeded)
            {
                return BadRequest("Error confirming your email. Please try again or contact support.");
            }

            _logger.LogInformation("\n\n\n\nEmail confirmed successfully for user {dto.UserId} \n\n\n\n", dto.UserId);
            return Ok(new { message = "Email confirmed successfully. You can now log in to your account." });
        }





        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {
            _logger.LogInformation("\n\n\n\nAttempting to log in user: {userDto.Username}\n\n\n\n", userDto.Username);

            try
            {
                var user = await _userManager.FindByNameAsync(userDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("\n\n\n\nFailed login attempt for invalid username: {userDto.Username}\n\n\n\n", userDto.Username);
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogWarning("\n\n\n\nLogin attempt for unconfirmed email: {user.Email}\n\n\n\n", user.Email);
                    return BadRequest(new { message = "Please confirm your email before logging in." });
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
                    _logger.LogWarning("\n\n\n\nUser account locked out: {user.UserName}\n\n\n\n", user.UserName);
                    return StatusCode(StatusCodes.Status423Locked, new { message = "Account is locked. Please try again later." });
                }

                _logger.LogWarning("\n\n\n\nFailed login attempt for user: {user.UserName}\n\n\n\n", user.UserName);
                return Unauthorized(new { message = "Invalid username or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "\n\n\n\nUnexpected error during login\n\n\n\n");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error has occurred. Please try again later.",
                    errorCode = "UNEXPECTED_ERROR"
                });
            }
        }

        private async Task<IActionResult> CompleteLoginAsync(User user)
        {
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("\n\n\n\nUser {user.UserName} logged in successfully\n\n\n\n", user.UserName);
            return Ok(new { requiredTwoFactor = false, message = "Logged in successfully" });
        }

        private async Task<IActionResult> InitiateTwoFactorAuthenticationAsync(User user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailService.TrySendEmailAsync(user.Email, "Your 2FA Code", $"Your login code is: {token}");

            _logger.LogInformation("\n\n\n\n2FA initiated for user: {user.UserName}\n\n\n\n", user.UserName);
            return Ok(new { requiresTwoFactor = true, message = "2FA code sent to your email." });
        }

        [HttpPost("VerifyTwoFactorCode")]
        public async Task<IActionResult> VerifyTwoFactorCode(TwoFactorVerificationDto twoFactorDto)
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO VERIFY 2FA\n\n\n\n");
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return BadRequest("Invalid request");
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", twoFactorDto.Code, twoFactorDto.RememberMe, twoFactorDto.RememberBrowser);
            if (result.Succeeded)
            {
                _logger.LogInformation("\n\n\n\n2FA WORKED FOR {user.UserName}\n\n\n\n", user.UserName);
                return await CompleteLoginAsync(user);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("\n\n\n\nUser account locked out during 2FA: {user.UserName}\n\n\n\n", user.UserName);
                return StatusCode(StatusCodes.Status423Locked, new { message = "Account is locked. Please try again later." });
            }
            else
            {
                _logger.LogWarning("\n\n\n\nInvalid 2FA code attempt for user: {user.UserName}\n\n\n\n", user.UserName);
                return BadRequest("Invalid verification code");
            }
        }

        // POST: /api/Users/Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("\n\n\nLogging user out!!!\n\n\n");
            return Ok(new { message = "Logged out successfully" });
        }


        // GET: /api/Users/CheckSession
        [HttpGet("CheckSession")]
        public async Task<IActionResult> CheckSession()
        {
            _logger.LogInformation("\n\n\n\nCHECKING SESSION\n\n\n\n");
            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    _logger.LogInformation("\n\n\n\nUSER FOUND: {user.UserName}, {user.Id}\n\n\n\n", user.UserName, user.Id);

                    return Ok(new
                    {
                        isAuthenticated = true,
                        userId = user.Id,
                        username = user.UserName,
                        email = user.Email,
                    });
                }
            }
            _logger.LogInformation("\n\n\n\nNO AUTHENTICATION FOUND\n\n\n\n");
            return Ok(new { isAuthenticated = false });
        }

        // POST: /api/Users/ChangePassword
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {

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
                _logger.LogInformation("\n\n\n\nValidation Problems\n\n\n\n");
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(ModelState);
            }

            // Optional: Sign in the user again to update the authentication cookie
            await _signInManager.RefreshSignInAsync(user);

            _logger.LogInformation("\n\n\n\nUser {user.UserName} successfully changed their password.\n\n\n\n", user.UserName);

            return Ok("Your password has been changed successfully.");
        }

        public class ForgotPasswordDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO HANDLE FORGOT PASSWORD WITH EMAIL: {forgotPasswordDto.Email}\n\n\n\n", forgotPasswordDto.Email);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }


            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                _logger.LogInformation("\n\n\n\nFAKE USER EMAIL: {forgotPasswordDto.Email}\n\n\n\n", forgotPasswordDto.Email);
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
                _logger.LogWarning("\n\n\n\nFailed to send password reset email to user {user.Id}\n\n\n\n", user.Id);
                return StatusCode(500, "Failed to send password reset email. Please try again later.");
            }

            return Ok("If your email is registered and confirmed, you will receive a password reset link shortly.");
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO RESET PASSWORD\n\n\n\n");
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }


            var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
            if (user == null)
            {
                _logger.LogInformation("\n\n\n\nUSER NOT FOUND IN RESET\n\n\n\n");
                // Don't reveal that the user does not exist
                return Ok("If your account exists, your password has been reset.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("\n\n\n\nPassword reset successfully for user {user.Id}\n\n\n\n", user.Id);
                return Ok("Your password has been reset successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogInformation("\n\n\n\nERRORS CAME UP WHEN RESETTING PASSWORD\n\n\n\n");
            return ValidationProblem(ModelState);
        }


        // POST: api/Users/DeleteAccount
        [Authorize]
        [HttpPost("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(DeleteAccountDto deleteAccountDto)
        {
            _logger.LogInformation("\n\n\n\nAttemping to delete account\n\n\n\n");
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
                    _logger.LogError("\n\n\n\nFailed to delete user: {user.UserName} (ID: {user.Id})\n\n\n\n", user.UserName, user.Id);
                    // You can include error details from deleteResult.Errors if they provide useful context
                    return StatusCode(500, "Failed to delete user. Please try again later.");
                }

                // Log the account deletion
                _logger.LogInformation("\n\n\n\nUser account deleted: {user.UserName} (ID: {user.Id})\n\n\n\n", user.UserName, user.Id);

                // Sign out the user
                await _signInManager.SignOutAsync();

                return Ok("Your account has been successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "\n\n\n\nError deleting user account: {user.UserName} (ID: {user.Id})\n\n\n\n", user.UserName, user.Id);
                return StatusCode(500, "An error occurred while deleting your account. Please try again later.");
            }
        }

        [Authorize]
        [HttpGet("stats")]
        public async Task<ActionResult<UserStatsDto>> GetUserStats()
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO GET USER STATS\n\n\n\n");
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogInformation("\n\n\n\nUSER NOT FOUND\n\n\n\n");
                return NotFound("User not found.");
            }

            Dictionary<int, string> rankMap = new Dictionary<int, string>
            {
                { 0, "Newbie" },
                { 1, "Novice" },
                { 2, "Amateur" },
                { 3, "Talented" },
                { 4, "Pro" }
            };


            // Convert rank from enum value
            var rank = user.Rank.ToString() == "Newbie" ? "Newbie" : rankMap[(int)user.Rank];





            var userStats = new UserStatsDto
            {
                Username = user.UserName,
                TotalScore = user.TotalScore,
                Rank = rank,
                CompletedProblems = user.CompletedProblems,
                AttemptedProblems = user.AttemptedProblems,
                ProfilePictureURL = user.ProfilePictureURL,
                RegistrationDate = user.RegistrationDate
            };

            _logger.LogInformation("\n\n\n\nSUCCESSFULLY RETURNING STATS\n\n\n\n");
            return userStats;
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











    }
}
