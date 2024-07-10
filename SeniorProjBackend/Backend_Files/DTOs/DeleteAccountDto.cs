using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class DeleteAccountDto
    {
        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^DELETE$", ErrorMessage = "Please type 'DELETE' to confirm account deletion.")]
        public string ConfirmDeletion { get; set; }
    }
}
