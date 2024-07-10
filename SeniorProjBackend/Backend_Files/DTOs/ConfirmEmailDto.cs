using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Confirmation token is required.")]
        public string Token { get; set; }
    }
}
