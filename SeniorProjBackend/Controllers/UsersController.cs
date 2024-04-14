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

namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly OurDbContext _context;

        public UsersController(OurDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService=tokenService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize] // Apply JWT authentication middleware
        public async Task<ActionResult<User>> GetUser(int id)
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
            // check if the username is already taken
            bool userExists = await _context.Users.AnyAsync(u => u.Username == userDto.Username);
            if (userExists)
            {
                return BadRequest("Username is already taken.");
            }

            // check if the email address is already taken
            bool emailExists = await _context.Users.AnyAsync(u => u.EmailAddress == userDto.EmailAddress);
            if (emailExists)
            {
                return BadRequest("Email Address is already taken.");
            }

            // hash the password
            var hasher = new PasswordHasher();
            var hashedPassword = hasher.HashPassword(userDto.Password);

            // create a new User entity
            var newUser = new User
            {
                Username = userDto.Username,
                PasswordHash = hashedPassword, // store the hashed password
                EmailAddress = userDto.EmailAddress
            };

            // add user to the database context
            _context.Users.Add(newUser);


            await _context.SaveChangesAsync();



            var token = _tokenService.GenerateToken(newUser); // Use the injected _tokenService

            return Ok(new { token }); // Return the JWT in the response
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(UserLoginDto userDto)
        {
            // retrieve the user from the database
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // verify the password hash
            var hasher = new PasswordHasher();
            var passwordIsValid = hasher.VerifyPassword(user.PasswordHash, userDto.Password);
            if (!passwordIsValid)
            {
                return Unauthorized("Invalid username or password.");
            }

            user.LastActiveDate =  DateTime.UtcNow;

            // generate a JWT
            var token = _tokenService.GenerateToken(user);

            // Return the JWT in the response
            return Ok(new { token = token });
        }


    }
}
