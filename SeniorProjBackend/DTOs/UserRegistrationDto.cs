using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Username is Required")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username must be alphanumeric and can include underscores.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [MinLength(6, ErrorMessage = "The Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password Confirmation is Required")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ComparePassword { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string EmailAddress { get; set; }

    }
}
