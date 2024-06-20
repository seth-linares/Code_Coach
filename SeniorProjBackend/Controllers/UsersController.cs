using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Encryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly OurDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(OurDbContext context, ITokenService tokenService, ILogger<UsersController> logger, PasswordHasher<User> passwordHasher)
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
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



        [HttpPost("Register")]
        public async Task<ActionResult<string>> RegisterUser(UserRegistrationDto userDto)
        {
            var existingUser = await _context.Users
                .Where(u => u.Username == userDto.Username || u.EmailAddress == userDto.EmailAddress)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Username == userDto.Username)
                {
                    return BadRequest(new { message = "Username is already taken." });
                }
                else if(existingUser.EmailAddress == userDto.EmailAddress)
                {
                    return BadRequest(new { message = "Email Address is already taken." });
                }
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
                return StatusCode(500, new { message = $"An error occured while creating the user: {ex.Message}" });
            }



            var token = _tokenService.GenerateToken(newUser); // Use the injected _tokenService

            // Return the JWT in the response
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, new { token }); 
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(UserLoginDto userDto)
        {
            // retrieve the user from the database
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // verify the password hash
            var passwordIsValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
            if (passwordIsValid == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }


            /*
             * If we know the password used in the Dto was legit but old, we can just
             * take it from the Dto and then hash it and store the new password
             */
            else if (passwordIsValid == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);
            }

            user.LastActiveDate =  DateTime.UtcNow;



            await _context.SaveChangesAsync();





            // generate a JWT
            var token = _tokenService.GenerateToken(user);

            // Return the JWT in the response
            return Ok(new { token });
        }


    }
}
