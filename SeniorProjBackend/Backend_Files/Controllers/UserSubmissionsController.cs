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
    public class UserSubmissionsController : ControllerBase
    {
        private readonly OurDbContext _context;

        public UserSubmissionsController(OurDbContext context)
        {
            _context = context;
        }

        // GET: api/UserSubmissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSubmission>>> GetUserSubmissions()
        {
            return await _context.UserSubmissions.ToListAsync();
        }

        // GET: api/UserSubmissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSubmission>> GetUserSubmission(int id)
        {
            var userSubmission = await _context.UserSubmissions.FindAsync(id);

            if (userSubmission == null)
            {
                return NotFound();
            }

            return userSubmission;
        }

        // PUT: api/UserSubmissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSubmission(int id, UserSubmission userSubmission)
        {
            if (id != userSubmission.SubmissionID)
            {
                return BadRequest();
            }

            _context.Entry(userSubmission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSubmissionExists(id))
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

        // POST: api/UserSubmissions
        [HttpPost]
        public async Task<ActionResult<UserSubmission>> PostUserSubmission(UserSubmission userSubmission)
        {
            _context.UserSubmissions.Add(userSubmission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserSubmission", new { id = userSubmission.SubmissionID }, userSubmission);
        }

        // DELETE: api/UserSubmissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSubmission(int id)
        {
            var userSubmission = await _context.UserSubmissions.FindAsync(id);
            if (userSubmission == null)
            {
                return NotFound();
            }

            _context.UserSubmissions.Remove(userSubmission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserSubmissionExists(int id)
        {
            return _context.UserSubmissions.Any(e => e.SubmissionID == id);
        }
    }
}
