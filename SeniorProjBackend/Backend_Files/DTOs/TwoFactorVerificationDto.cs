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
}
