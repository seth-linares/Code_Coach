using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
