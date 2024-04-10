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

namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly OurDbContext _context;

        public UsersController(OurDbContext context)
        {
            _context = context;
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



        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<ActionResult<User>> RegisterUser(UserRegistrationDto userDto)
        {
            // Validate request data and ensure username/email uniqueness
            // Hash the password
            // Create a new User entity and save it to the database
            // Return a success response

            // Check if the username is already taken
            bool user_exists = await _context.Users.AnyAsync(u => u.Username == userDto.Username);



            return await default(Task<ActionResult<User>>);
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginUser(UserLoginDto userDto)
        {
            // Validate request data and retrieve the user from the database
            // Verify the password
            // Generate a JWT and return it in the response
            return await default(Task<ActionResult<string>>);
            
        }

    }
}
