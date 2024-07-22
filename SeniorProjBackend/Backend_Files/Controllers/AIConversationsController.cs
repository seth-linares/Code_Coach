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

        [HttpPost("test")]
        public async Task<IActionResult> TestChatGPTService()
        {
            _logger.LogInformation("\n\n\n\n\nStarting ChatGPT service test\n\n\n\n\n");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("\n\n\n\n\nUnauthorized access attempt\n\n\n\n\n");
                return Unauthorized();
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                _logger.LogWarning($"\n\n\n\n\nNo active API key found for user {user.Id}\n\n\n\n\n");
                return BadRequest("No active API key found");
            }

            _logger.LogInformation($"\n\n\n\n\nUsing API key: {activeApiKey.KeyName}\n\n\n\n\n");

            string testMessage = "SSBhbSB0cnlpbmcgdG8gc29sdmUgdGhpcyBjb2RpbmcgYmF0IGFuZCBuZWVkIGhlbHAgdW5kZXJzdGFuZGluZyB0aGUgcHJvbXB0LiBDYW4geW91IGV4cGxhaW4gaXQgdG8gbWU/OgpHaXZlbiBhIHN0cmluZywgcmV0dXJuIGEgc3RyaW5nIGxlbmd0aCAyIG1hZGUgb2YgaXRzIGZpcnN0IDIgY2hhcnMuIElmIHRoZSBzdHJpbmcgbGVuZ3RoIGlzIGxlc3MgdGhhbiAyLCB1c2UgJ0AnIGZvciB0aGUgbWlzc2luZyBjaGFycy4KCmF0Rmlyc3QoImhlbGxvIikg4oaSICJoZSIKYXRGaXJzdCgiaGkiKSDihpIgImhpIgphdEZpcnN0KCJoIikg4oaSICJoQCIK";
            _logger.LogInformation($"\n\n\n\n\nTest message: {testMessage}\n\n\n\n\n");

            try
            {
                var chatGPTResponse = await _chatGPTService.SendMessage(activeApiKey.KeyValue, testMessage);

                activeApiKey.UsageCount++;
                activeApiKey.LastUsedAt = DateTime.UtcNow;

                _logger.LogInformation($"\n\n\n\n\nChatGPT Response:\n{chatGPTResponse.Choices[0].Message.Content}\n\n\n\n\n");

                _logger.LogInformation($"\n\n\n\n\nUsage Statistics:\nPrompt Tokens: {chatGPTResponse.Usage.PromptTokens}\nCompletion Tokens: {chatGPTResponse.Usage.CompletionTokens}\nTotal Tokens: {chatGPTResponse.Usage.TotalTokens}\n\n\n\n\n");

                return Ok(new 
                { 
                    Message = chatGPTResponse.Choices[0].Message.Content,
                    Usage = chatGPTResponse.Usage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"\n\n\n\n\nError occurred while testing ChatGPT service: {ex.Message}\n\nStack Trace: {ex.StackTrace}\n\n\n\n\n");
                return StatusCode(500, "An error occurred while testing the ChatGPT service. Please check the logs for more information.");
            }
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
            _logger.LogInformation($"\n\n\n\nReceived ChatGPT request for ProblemId: {request.ProblemId}\n\n\n\n");
            _logger.LogInformation($"\n\n\n\nUSER MESSAGE IN BASE64: {request.Message}\n\n\n\n");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning($"\n\n\n\nUnauthorized access attempt\n\n\n\n");
                return Unauthorized();
            }

            var activeApiKey = await _context.APIKeys
                .FirstOrDefaultAsync(k => k.UserId == user.Id && k.IsActive);

            if (activeApiKey == null)
            {
                _logger.LogWarning($"\n\n\n\nNo active API key found for user: {user.Id}\n\n\n\n");
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
                _logger.LogInformation($"\n\n\n\nStarting new conversation for user: {user.Id}, ProblemId: {request.ProblemId}\n\n\n\n");
            }
            else
            {
                // Continue existing conversation
                conversation = await _context.AIConversations
                    .Include(c => c.Messages)
                    .FirstOrDefaultAsync(c => c.ConversationID == request.ConversationId && c.UserId == user.Id);

                if (conversation == null)
                {
                    _logger.LogWarning($"\n\n\n\nConversation not found. ConversationId: {request.ConversationId}, UserId: {user.Id}\n\n\n\n");
                    return NotFound("Conversation not found");
                }
                _logger.LogInformation($"\n\n\n\nContinuing conversation: {conversation.ConversationID} for user: {user.Id}\n\n\n\n");
            }

            // Fetch the problem description
            var problem = await _context.Problems
                .FirstOrDefaultAsync(p => p.ProblemID == request.ProblemId);

            if (problem == null)
            {
                _logger.LogWarning($"\n\n\n\nProblem not found. ProblemId: {request.ProblemId}\n\n\n\n");
                return NotFound("Problem not found");
            }

            try
            {
                _logger.LogInformation($"\n\n\n\n\n\n\n\nTRANSLATED!! {System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(problem.Description))}\n\n\n\n");
                _logger.LogInformation($"\n\n\n\nTRANSLATED!! {System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Message))}\n\n\n\n\n\n\n\n\n");
                // Combine problem description and user message
                string combinedMessage = $"Problem Description (Base64 Encoded): {System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(problem.Description))}\n\nUser Message: {System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Message))}";

                _logger.LogInformation($"\n\n\n\nSending message to ChatGPT service. Combined message length: {combinedMessage.Length}\n\n\n\n");

                // Send the combined message to the ChatGPT service
                var chatGPTResponse = await _chatGPTService.SendMessage(activeApiKey.KeyValue, combinedMessage);

                if (chatGPTResponse?.Choices == null || chatGPTResponse.Choices.Count == 0)
                {
                    _logger.LogError($"\n\n\n\nReceived invalid response from ChatGPT service\n\n\n\n");
                    return StatusCode(500, "Received an invalid response from the ChatGPT service");
                }

                _logger.LogInformation($"\n\n\n\nReceived valid response from ChatGPT service. Response length: {chatGPTResponse.Choices[0].Message?.Content?.Length ?? 0}\n\n\n\n");

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

                _logger.LogInformation($"\n\n\n\nSuccessfully processed ChatGPT response. ConversationId: {conversation.ConversationID}, TotalTokens: {conversation.TotalTokens}\n\n\n\n");

                return Ok(new
                {
                    ConversationId = conversation.ConversationID,
                    Message = assistantMessage.Content
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"\n\n\n\nUnauthorized access: {ex.Message}\n\n\n\n");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"\n\n\n\nException: {ex}\nError occurred while processing ChatGPT response\n\n\n\n");
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
