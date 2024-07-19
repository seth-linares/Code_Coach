using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProjBackend.Data;
using SeniorProjBackend.DTOs;

namespace SeniorProjBackend.Controllers
{
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateAPIKey(CreateAPIKeyRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKey = new APIKey
            {
                UserId = user.Id,
                KeyName = request.KeyName,
                KeyValue = request.KeyValue,
                IsActive = false,
                CreatedAt = DateTimeOffset.UtcNow,
                UsageCount = 0
            };

            _context.APIKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"\n\n\n\nAPI Key created for user {user.Id}\n\n\n\n");
            return Ok(new { Message = "API Key created successfully", APIKeyID = apiKey.APIKeyID });
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAPIKeys()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

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

            return Ok(apiKeys);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAPIKey(UpdateAPIKeyRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKey = await _context.APIKeys.FindAsync(request.APIKeyID);
            if (apiKey == null || apiKey.UserId != user.Id)
            {
                return NotFound("API Key not found or does not belong to the user");
            }

            apiKey.KeyName = request.KeyName;
            apiKey.KeyValue = request.KeyValue;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"\n\n\n\nAPI Key {apiKey.APIKeyID} updated for user {user.Id}\n\n\n\n");
            return Ok(new { Message = "API Key updated successfully" });
        }

        public class APIKeyIdDto
        {
            public int KeyID { get; set; }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAPIKey(APIKeyIdDto apiKeyIdDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKey = await _context.APIKeys.FindAsync(apiKeyIdDto.KeyID);
            if (apiKey == null || apiKey.UserId != user.Id)
            {
                return NotFound("API Key not found or does not belong to the user");
            }

            _context.APIKeys.Remove(apiKey);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"\n\n\n\nAPI Key {apiKeyIdDto.KeyID} deleted for user {user.Id}\n\n\n\n");
            return Ok(new { Message = "API Key deleted successfully" });
        }

        [HttpPut("set-active")]
        public async Task<IActionResult> SetActiveAPIKey(SetActiveAPIKeyRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var apiKeys = await _context.APIKeys.Where(k => k.UserId == user.Id).ToListAsync();
            var activeKey = apiKeys.FirstOrDefault(k => k.APIKeyID == request.APIKeyID);

            if (activeKey == null)
            {
                return NotFound("API Key not found or does not belong to the user");
            }

            foreach (var key in apiKeys)
            {
                key.IsActive = (key.APIKeyID == request.APIKeyID);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"\n\n\n\nAPI Key {request.APIKeyID} set as active for user {user.Id}\n\n\n\n");
            return Ok(new { Message = "Active API Key set successfully" });
        }
    }
}
