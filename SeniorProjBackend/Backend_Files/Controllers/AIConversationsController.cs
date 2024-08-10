using Microsoft.AspNetCore.Authorization;
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



        [HttpPost("ChatGPT")]
        public async Task<IActionResult> ChatGPT(ChatGPTRequest request)
        {
            _logger.LogInformation("\n\n\n\nReceived ChatGPT request for ProblemId: {ProblemID}\n\n\n\n", request.ProblemId);
            _logger.LogInformation("\n\n\n\nUSER MESSAGE IN BASE64: {Message}\n\n\n\n", request.Message);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("\n\n\n\nUnauthorized access attempt\n\n\n\n");
                return Unauthorized();
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                _logger.LogWarning("\n\n\n\nNo active API key found for user: {Id}\n\n\n\n", user.Id);
                return BadRequest("No active API key found. Please set an active API key before starting a conversation.");
            }

            AIConversation conversation;
            if (request.ConversationId == null)
            {
                // Start a new conversation
                conversation = new AIConversation
                {
                    UserId = user.Id,
                    ProblemID = request.ProblemId,
                    StartTime = DateTimeOffset.UtcNow,
                    Model = "gpt-4o-mini"
                };
                _context.AIConversations.Add(conversation);
                _logger.LogInformation("\n\n\n\nStarting new conversation for user: {Id}, ProblemId: {ProblemId}\n\n\n\n", user.Id, request.ProblemId);
            }
            else
            {
                // Continue existing conversation
                conversation = await _context.AIConversations
                    .Include(c => c.Messages)
                    .FirstOrDefaultAsync(c => c.ConversationID == request.ConversationId && c.UserId == user.Id);

                if (conversation == null)
                {
                    _logger.LogWarning("\n\n\n\nConversation not found. ConversationId: {ConversationId}, UserId: {Id}\n\n\n\n", request.ConversationId, user.Id);
                    return NotFound("Conversation not found");
                }
                _logger.LogInformation("\n\n\n\nContinuing conversation: {ConversationID} for user: {Id}\n\n\n\n", conversation.ConversationID, user.Id);
            }

            // Fetch the problem description
            var problem = await _context.Problems
                .FirstOrDefaultAsync(p => p.ProblemID == request.ProblemId);

            if (problem == null)
            {
                _logger.LogWarning("\n\n\n\nProblem not found. ProblemId: {ProblemId}\n\n\n\n", request.ProblemId);
                return NotFound("Problem not found");
            }

            try
            {
                string description = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(problem.Description));
                string message = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Message));

                _logger.LogInformation("\n\n\n\n\n\n\n\nTRANSLATED!! {Description}\n\n\n\n", description);
                _logger.LogInformation("\n\n\n\nTRANSLATED!! {Message}\n\n\n\n\n\n\n\n\n", message);
                // Combine problem description and user message
                string combinedMessage = $"Problem Description: {description}\n\nUser Message: {message}";

                _logger.LogInformation("\n\n\n\nSending message to ChatGPT service. Combined message length: {Length}\n\n\n\n", combinedMessage.Length);

                // Send the combined message to the ChatGPT service
                var chatGPTResponse = await _chatGPTService.SendMessage(activeApiKey.KeyValue, combinedMessage);

                if (chatGPTResponse?.Choices == null || chatGPTResponse.Choices.Count == 0)
                {
                    _logger.LogError("\n\n\n\nReceived invalid response from ChatGPT service\n\n\n\n");
                    return StatusCode(500, "Received an invalid response from the ChatGPT service");
                }

                _logger.LogInformation("\n\n\n\nReceived valid response from ChatGPT service. Response length: {Length}\n\n\n\n",
                    chatGPTResponse.Choices[0].Message?.Content?.Length ?? 0);

                var userMessage = new AIMessage
                {
                    ConversationID = conversation.ConversationID,
                    Content = request.Message,
                    Role = "user",
                    Timestamp = DateTimeOffset.UtcNow,
                    PromptTokens = chatGPTResponse.Usage?.PromptTokens ?? 0
                };

                var assistantMessage = new AIMessage
                {
                    ConversationID = conversation.ConversationID,
                    Content = chatGPTResponse.Choices[0].Message?.Content ?? "No response content",
                    Role = "assistant",
                    Timestamp = DateTimeOffset.UtcNow,
                    CompletionTokens = chatGPTResponse.Usage?.CompletionTokens ?? 0
                };

                conversation.Messages ??= new List<AIMessage>();
                conversation.Messages.Add(userMessage);
                conversation.Messages.Add(assistantMessage);
                conversation.TotalTokens += chatGPTResponse.Usage?.TotalTokens ?? 0;

                activeApiKey.UsageCount++;
                activeApiKey.LastUsedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("\n\n\n\nSuccessfully processed ChatGPT response. ConversationId: {ConversationID}, TotalTokens: {TotalTokens}\n\n\n\n",
                    conversation.ConversationID, conversation.TotalTokens);

                return Ok(new
                {
                    ConversationId = conversation.ConversationID,
                    Message = assistantMessage.Content
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("\n\n\n\nUnauthorized access: {Message}\n\n\n\n", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "\n\n\n\nError occurred while processing ChatGPT response\n\n\n\n");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("GetConversationHistory")]
        public async Task<IActionResult> GetConversationHistory(RequestId conversationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversation = await _context.AIConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId.Id && c.UserId == user.Id);

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
                Messages = messages,
                TotalTokens = conversation.TotalTokens
            });
        }

        [HttpPost("ListConversations")]
        public async Task<IActionResult> ListConversations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var conversations = await _context.AIConversations
                .Where(c => c.UserId == user.Id)
                .OrderByDescending(c => c.StartTime)
                .Select(c => new ConversationListItemDto
                {
                    ConversationID = c.ConversationID,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    ProblemID = c.ProblemID,
                    MessageCount = c.Messages.Count,
                    TotalTokens = c.TotalTokens
                })
                .ToListAsync();


            return Ok(conversations);
        }

        [HttpPost("GetConversationsByProblem")]
        public async Task<IActionResult> GetConversationsByProblem(ConversationsByProblemRequest request)
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
                .Select(c => new ConversationListItemDto
                {
                    ConversationID = c.ConversationID,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    MessageCount = c.Messages.Count,
                    TotalTokens = c.TotalTokens
                })
                .ToListAsync();

            return Ok(conversations);
        }
    }

}
