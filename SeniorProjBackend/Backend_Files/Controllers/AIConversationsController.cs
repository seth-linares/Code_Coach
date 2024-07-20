using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;
using SeniorProjBackend.Middleware;

namespace SeniorProjBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AIConversationsController : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AIConversationsController> _logger;
        private readonly IChatGPTService _chatGPTService;

        public AIConversationsController(OurDbContext context, UserManager<User> userManager, ILogger<AIConversationsController> logger, IChatGPTService chatGPTService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _chatGPTService = chatGPTService;
        }

        // GET: api/AIConversations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AIConversation>>> GetAIConversations()
        {
            return await _context.AIConversations.ToListAsync();
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation(StartConversationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                return BadRequest("No active API key found. Please set an active API key before starting a conversation.");
            }

            var conversation = new AIConversation
            {
                UserId = user.Id,
                ProblemID = request.ProblemId,
                LanguageID = request.LanguageId,
                StartTime = DateTimeOffset.UtcNow,
                Model = "gpt-4o-mini"
            };

            _context.AIConversations.Add(conversation);
            await _context.SaveChangesAsync();

            return Ok(new { ConversationId = conversation.ConversationID });
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage(SendMessageRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversation = await _context.AIConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationID == request.ConversationId && c.UserId == user.Id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                return BadRequest("No active API key found");
            }

            var chatGPTResponse = await _chatGPTService.SendMessage(activeApiKey.KeyValue, conversation.Messages, request.Message);

            var userMessage = new AIMessage
            {
                ConversationID = conversation.ConversationID,
                Content = request.Message,
                Role = "user",
                Timestamp = DateTimeOffset.UtcNow,
                PromptTokens = chatGPTResponse.Usage.PromptTokens
            };

            var assistantMessage = new AIMessage
            {
                ConversationID = conversation.ConversationID,
                Content = chatGPTResponse.Choices[0].Message.Content,
                Role = "assistant",
                Timestamp = DateTimeOffset.UtcNow,
                CompletionTokens = chatGPTResponse.Usage.CompletionTokens
            };

            conversation.Messages.Add(userMessage);
            conversation.Messages.Add(assistantMessage);
            conversation.TotalTokens += chatGPTResponse.Usage.TotalTokens;

            activeApiKey.UsageCount++;
            activeApiKey.LastUsedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { Message = assistantMessage.Content });
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationHistory(int conversationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversation = await _context.AIConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId && c.UserId == user.Id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var messages = conversation.Messages.Select(m => new MessageDto
            {
                Role = m.Role,
                Content = m.Content,
                Timestamp = m.Timestamp,
                Tokens = m.PromptTokens + m.CompletionTokens
            }).ToList();

            return Ok(new ConversationHistoryDto
            {
                ConversationID = conversation.ConversationID,
                StartTime = conversation.StartTime,
                EndTime = conversation.EndTime,
                ProblemID = conversation.ProblemID,
                LanguageID = conversation.LanguageID,
                Messages = messages,
                TotalTokens = conversation.TotalTokens
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversations = await _context.AIConversations
                .Where(c => c.UserId == user.Id)
                .OrderByDescending(c => c.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ConversationListItemDto
                {
                    ConversationID = c.ConversationID,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    ProblemID = c.ProblemID,
                    LanguageID = c.LanguageID,
                    MessageCount = c.Messages.Count,
                    TotalTokens = c.TotalTokens
                })
                .ToListAsync();

            var totalCount = await _context.AIConversations.CountAsync(c => c.UserId == user.Id);

            return Ok(new PaginatedResponse<ConversationListItemDto>
            {
                Items = conversations,
                TotalCount = totalCount,
                PageCount = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = page,
                PageSize = pageSize
            });
        }

        [HttpPost("by-problem")]
        public async Task<IActionResult> GetConversationsByProblem([FromBody] ConversationsByProblemRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var query = _context.AIConversations
                .Where(c => c.UserId == user.Id && c.ProblemID == request.ProblemId)
                .OrderByDescending(c => c.StartTime);

            var totalCount = await query.CountAsync();

            var conversations = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ConversationListItemDto
                {
                    ConversationID = c.ConversationID,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    LanguageID = c.LanguageID,
                    MessageCount = c.Messages.Count,
                    TotalTokens = c.TotalTokens
                })
                .ToListAsync();

            return Ok(new PaginatedResponse<ConversationListItemDto>
            {
                Items = conversations,
                TotalCount = totalCount,
                PageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
                CurrentPage = request.Page,
                PageSize = request.PageSize
            });
        }

        [HttpPost("{conversationId}/end")]
        public async Task<IActionResult> EndConversation(int conversationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversation = await _context.AIConversations
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId && c.UserId == user.Id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            conversation.EndTime = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Conversation ended successfully" });
        }

        [HttpGet("{conversationId}/summary")]
        public async Task<IActionResult> GetConversationSummary(int conversationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversation = await _context.AIConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId && c.UserId == user.Id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                return BadRequest("No active API key found");
            }

            var summaryRequest = "Please provide a brief summary of our conversation so far, highlighting the main topics and key points discussed.";
            var chatGPTResponse = await _chatGPTService.SendMessage(activeApiKey.KeyValue, conversation.Messages, summaryRequest);

            return Ok(new { Summary = chatGPTResponse.Choices[0].Message.Content });
        }

        [HttpGet("{conversationId}/suggested-questions")]
        public async Task<IActionResult> GetSuggestedQuestions(int conversationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversation = await _context.AIConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId && c.UserId == user.Id);

            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                return BadRequest("No active API key found");
            }

            var suggestionsRequest = "Based on our conversation so far, what are 3 relevant follow-up questions the user might want to ask to deepen their understanding of the topics we've discussed?";
            var chatGPTResponse = await _chatGPTService.SendMessage(activeApiKey.KeyValue, conversation.Messages, suggestionsRequest);

            return Ok(new { SuggestedQuestions = chatGPTResponse.Choices[0].Message.Content });
        }
    }

}
