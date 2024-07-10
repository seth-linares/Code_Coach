using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;

namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPreferencesController : ControllerBase
    {
        private readonly OurDbContext _context;

        public UserPreferencesController(OurDbContext context)
        {
            _context = context;
        }

        // GET: api/UserPreferences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPreference>>> GetUserPreferences()
        {
            return await _context.UserPreferences.ToListAsync();
        }

        // GET: api/UserPreferences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPreference>> GetUserPreference(int id)
        {
            var userPreference = await _context.UserPreferences.FindAsync(id);

            if (userPreference == null)
            {
                return NotFound();
            }

            return userPreference;
        }

        // PUT: api/UserPreferences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPreference(int id, UserPreference userPreference)
        {
            if (id != userPreference.UserPreferenceID)
            {
                return BadRequest();
            }

            _context.Entry(userPreference).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPreferenceExists(id))
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

        // POST: api/UserPreferences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserPreference>> PostUserPreference(UserPreference userPreference)
        {
            _context.UserPreferences.Add(userPreference);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserPreference", new { id = userPreference.UserPreferenceID }, userPreference);
        }

        // DELETE: api/UserPreferences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPreference(int id)
        {
            var userPreference = await _context.UserPreferences.FindAsync(id);
            if (userPreference == null)
            {
                return NotFound();
            }

            _context.UserPreferences.Remove(userPreference);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserPreferenceExists(int id)
        {
            return _context.UserPreferences.Any(e => e.UserPreferenceID == id);
        }
    }
}
