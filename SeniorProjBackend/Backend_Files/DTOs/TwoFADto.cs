using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class TwoFactorVerificationDto
    {
        [Required]
        public string Code { get; set; }

        public bool RememberMe { get; set; }

        public bool RememberBrowser { get; set; }
    }



    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Confirmation token is required.")]
        public string Token { get; set; }
    }


}
