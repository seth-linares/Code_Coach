using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;

namespace SeniorProjBackend.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class APIKeysController : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<APIKeysController> _logger;

        public APIKeysController(OurDbContext context, UserManager<User> userManager, ILogger<APIKeysController> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet("GetAPIKeys")]
        public async Task<ActionResult<IEnumerable<APIKey>>> GetAPIKeys()
        {
            return await _context.APIKeys.Include(k => k.User).ToListAsync();
        }

        // GET: api/APIKeys/ListAPIKeys
        [HttpGet("ListAPIKeys")]
        public async Task<IActionResult> ListAPIKeys()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            _logger.LogInformation("\n\n\n\nLISTING API KEYS\n\n\n\n");

            var apiKeys = await _context.APIKeys
                .Where(k => k.UserId == user.Id)
                .Select(k => new APIKeyListItemDto
                {
                    APIKeyID = k.APIKeyID,
                    KeyName = k.KeyName,
                    IsActive = k.IsActive,
                    CreatedAt = k.CreatedAt,
                    LastUsedAt = k.LastUsedAt,
                    UsageCount = k.UsageCount
                })
                .ToListAsync();

            _logger.LogInformation("\n\n\n\nAPI KEY COUNT: {Count}\n\n\n\n", apiKeys.Count);

            return Ok(apiKeys);
        }

        [HttpPost("GetAPIKeyUsage")]
        public async Task<IActionResult> GetAPIKeyUsage(RequestId apiKeyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKeyUsage = await _context.APIKeys
                .Where(k => k.UserId == user.Id && k.APIKeyID == apiKeyId.Id)
                .Select(k => new APIKeyUsageDto
                {
                    APIKeyID = k.APIKeyID,
                    KeyName = k.KeyName,
                    UsageCount = k.UsageCount,
                    TotalTokensUsed = k.User.AIConversations.Sum(c => c.TotalTokens),
                    LastUsedAt = k.LastUsedAt,
                    AverageTokensPerUse = k.UsageCount > 0 ?
                        (double)k.User.AIConversations.Sum(c => c.TotalTokens) / k.UsageCount : 0
                })
                .FirstOrDefaultAsync();

            if (apiKeyUsage == null)
            {
                return NotFound("API Key not found");
            }

            return Ok(apiKeyUsage);
        }

        [HttpPost("CreateAPIKey")]
        public async Task<IActionResult> CreateAPIKey(CreateAPIKeyRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Deactivate all existing keys
            var existingKeys = await _context.APIKeys.Where(k => k.UserId == user.Id).ToListAsync();
            foreach (var key in existingKeys)
            {
                key.IsActive = false;
            }
            _logger.LogInformation("\n\n\n\nDEACTIVATED KEYS\n\n\n\n");

            // Create and activate the new key
            var apiKey = new APIKey
            {
                UserId = user.Id,
                KeyName = request.KeyName,
                KeyValue = request.KeyValue,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UsageCount = 0
            };

            _context.APIKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            _logger.LogInformation("\n\n\n\nNew API Key created and set as active for user {Id}\n\n\n\n", user.Id);
            return Ok(new { Message = "API Key created successfully and set as active", APIKeyID = apiKey.APIKeyID });
        }



        [HttpPut("UpdateAPIKey")]
        public async Task<IActionResult> UpdateAPIKey(UpdateAPIKeyRequest request)
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO UPDATE KEY: {KeyName}, {APIKeyID}, value: {KeyValue}\n\n\n\n", request.KeyName, request.APIKeyID, request.KeyValue);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKey = await _context.APIKeys.FindAsync(request.APIKeyID);
            if (apiKey == null || apiKey.UserId != user.Id)
            {
                _logger.LogInformation("\n\n\n\nAPI Key not found or does not belong to the user\n\n\n\n");
                return NotFound("API Key not found or does not belong to the user");
            }

            apiKey.KeyName = request.KeyName;
            apiKey.KeyValue = request.KeyValue;

            await _context.SaveChangesAsync();

            _logger.LogInformation("\n\n\n\nAPI Key {APIKeyID} updated for user {Id}\n\n\n\n", apiKey.APIKeyID, user.Id);
            return Ok(new { Message = "API Key updated successfully" });
        }


        [HttpDelete("DeleteAPIKey")]
        public async Task<IActionResult> DeleteAPIKey(RequestId KeyID)
        {
            _logger.LogInformation("\n\n\n\nATTEMPTING TO DELETE KEY ID: {Id}\n\n\n\n", KeyID.Id);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKey = await _context.APIKeys.FindAsync(KeyID.Id);
            if (apiKey == null || apiKey.UserId != user.Id)
            {
                _logger.LogInformation("\n\n\n\nAPI Key not found or does not belong to the user\n\n\n\n");
                return NotFound("API Key not found or does not belong to the user");
            }

            bool wasActive = apiKey.IsActive;

            _context.APIKeys.Remove(apiKey);
            await _context.SaveChangesAsync();

            if (wasActive)
            {
                var anotherApiKey = await _context.APIKeys
                    .Where(k => k.UserId == user.Id)
                    .FirstOrDefaultAsync();

                if (anotherApiKey != null)
                {
                    anotherApiKey.IsActive = true;
                    _context.APIKeys.Update(anotherApiKey);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("\n\n\n\nAPI Key {APIKeyID} set to active for user {Id}\n\n\n\n", anotherApiKey.APIKeyID, user.Id);
                }
            }

            _logger.LogInformation("\n\n\n\nAPI Key {Id} deleted for user {Id}\n\n\n\n", KeyID.Id, user.Id);
            return Ok(new { Message = "API Key deleted successfully" });
        }




        [HttpPut("SetActiveAPIKey")]
        public async Task<IActionResult> SetActiveAPIKey(RequestId APIKeyID)
        {
            _logger.LogInformation("\n\n\n\nTRYING TO SET KEY ID: {Id} ACTIVE\n\n\n\n", APIKeyID.Id);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKeys = await _context.APIKeys.Where(k => k.UserId == user.Id).ToListAsync();
            var activeKey = apiKeys.FirstOrDefault(k => k.APIKeyID == APIKeyID.Id);

            if (activeKey == null)
            {
                _logger.LogInformation("\n\n\n\nAPI Key not found or does not belong to the user\n\n\n\n");
                return NotFound("API Key not found or does not belong to the user");
            }



            foreach (var key in apiKeys)
            {
                key.IsActive = false;
            }

            _logger.LogInformation("\n\n\n\nSETTING ACTIVE API KEY: {APIKeyID}\n\n\n\n", activeKey.APIKeyID);

            activeKey.IsActive = true;

            await _context.SaveChangesAsync();

            _logger.LogInformation("\n\n\n\nAPI Key {APIKeyID} set as active for user {Id}\n\n\n\n", APIKeyID.Id, user.Id);
            return Ok(new { Message = "Active API Key set successfully" });
        }
    }
}
