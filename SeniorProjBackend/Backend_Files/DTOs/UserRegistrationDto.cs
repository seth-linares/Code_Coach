using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username must contain only letters, numbers, and underscores. No spaces or special characters are allowed.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
        public string EmailAddress { get; set; }
    }
}