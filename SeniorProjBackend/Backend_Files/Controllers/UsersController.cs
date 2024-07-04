using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Encryption;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using NuGet.Protocol;



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

        // Might want to 
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

        
        
        // POST: api/Users -- DONT THINK WE WANT A POST OTHER THAN LOGIN
        //[HttpPost]
        //public async Task<ActionResult<User>> PostUser(User user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetUserById), new { id = user.UserID }, user);
        //}



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



        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser(UserRegistrationDto userDto)
        {
            _logger.LogInformation($"\n\n\n\n\nATTEMPTING TO REGISTER USER {userDto.Username}\n\n\n\n\n");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model State is invalid.");
                return ValidationProblem(ModelState);
            }

            var existingUser = await _context.Users
                .Where(u => u.Username == userDto.Username || u.EmailAddress == userDto.EmailAddress)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {

                // ModelState.AddModelError(KEY_OF_ERROR, VALUE_OF_ERROR):

                ModelState.AddModelError(
                    existingUser.Username == userDto.Username ? "Username" : "EmailAddress",
                    existingUser.Username == userDto.Username ? "Username already exists." : "Email address is already in use."
                );

        

                return ValidationProblem(ModelState); // REALLY DON'T LIKE THIS RESPONSE TYPE BUT ANNOYING TO CHANGE
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

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while creating the user.");
                ModelState.AddModelError("Database", $"An error occurred while creating the user: {ex.Message}");
                return ValidationProblem(ModelState);
            }



            var token = _tokenService.GenerateToken(newUser); // Use the injected _tokenService

            //var cookieOptions = new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true, // HTTPS NEEDS TRUE, FALSE FOR DEV
            //    SameSite = SameSiteMode.None,
            //    Domain = "localhost",
            //    Expires = DateTime.UtcNow.AddHours(1)
            //};

            // Return the JWT in the response
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, new { token }); 
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(UserLoginDto userDto)
        {
            _logger.LogInformation($"\n\n\n\n\nATTEMPTING TO LOG IN USER: {userDto.Username}\n\n\n\n\n");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model State is invalid.");
                return ValidationProblem(ModelState);
            }

            // retrieve the user from the database
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null)
            {
                _logger.LogWarning($"\n\n\n\nFailed login attempt for invalid username: {userDto.Username}\n\n\n\n");

                ModelState.AddModelError("Unauthorized", "Invalid username or password.");

                return ValidationProblem(ModelState);
            }

            // verify the password hash
            var passwordIsValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
            if (passwordIsValid == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning($"\n\n\n\nWRONG PASSWORD ATTEMPT FOR USER: \nUsername: {user.Username}\n\n\n\n\n");

                ModelState.AddModelError("Unauthorized", "Invalid username or password.");

                return ValidationProblem(ModelState);
            }


            /*
             * If we know the password used in the Dto was legit but old, we can just
             * take it from the Dto and then hash it and store the new password
             */
            else if (passwordIsValid == PasswordVerificationResult.SuccessRehashNeeded)
            {
                _logger.LogInformation($"\n\n\n\nUser (Username: {user.Username}) rehashing password\n\n\n\n");
                user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);
            }

            user.LastActiveDate =  DateTime.UtcNow;


            try
            {
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating user login information: {ex}");
                ModelState.AddModelError("Database", "An error occurred during login. Please try again.");
                return ValidationProblem(ModelState);
            }




            // generate a JWT
            var token = _tokenService.GenerateToken(user);

            _logger.LogInformation($"\n\n\n\nUser, {user.Username}, has been logged in.\n\n\n\n");
            // Return the JWT in the response
            return Ok(new { token });
        }


    }
}
