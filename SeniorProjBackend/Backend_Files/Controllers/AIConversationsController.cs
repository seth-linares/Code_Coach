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
    public class AIConversationsController : ControllerBase
    {
        private readonly OurDbContext _context;

        public AIConversationsController(OurDbContext context)
        {
            _context = context;
        }

        // GET: api/AIConversations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AIConversation>>> GetAIConversations()
        {
            return await _context.AIConversations.ToListAsync();
        }

        // GET: api/AIConversations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AIConversation>> GetAIConversation(int id)
        {
            var aIConversation = await _context.AIConversations.FindAsync(id);

            if (aIConversation == null)
            {
                return NotFound();
            }

            return aIConversation;
        }

        // PUT: api/AIConversations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAIConversation(int id, AIConversation aIConversation)
        {
            if (id != aIConversation.ConversationID)
            {
                return BadRequest();
            }

            _context.Entry(aIConversation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AIConversationExists(id))
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

        // POST: api/AIConversations
        [HttpPost]
        public async Task<ActionResult<AIConversation>> PostAIConversation(AIConversation aIConversation)
        {
            _context.AIConversations.Add(aIConversation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAIConversation", new { id = aIConversation.ConversationID }, aIConversation);
        }

        // DELETE: api/AIConversations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAIConversation(int id)
        {
            var aIConversation = await _context.AIConversations.FindAsync(id);
            if (aIConversation == null)
            {
                return NotFound();
            }

            _context.AIConversations.Remove(aIConversation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AIConversationExists(int id)
        {
            return _context.AIConversations.Any(e => e.ConversationID == id);
        }
    }
}
