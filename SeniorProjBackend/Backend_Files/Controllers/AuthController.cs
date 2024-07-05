using Microsoft.AspNetCore.Mvc;

namespace SeniorProjBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // GET: api/Auth/validate
        [HttpGet("validate")]
        public IActionResult ValidateSession()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Status = "Invalid", Message = "User is not authenticated." });
            }

            return Ok(new { Status = "Valid" });
        }
    }
}
