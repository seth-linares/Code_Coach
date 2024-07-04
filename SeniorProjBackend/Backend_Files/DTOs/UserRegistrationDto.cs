using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class UserRegistrationDto
    {
        /*
         * Username property
         * Required: Ensures the field is not null or empty
         * StringLength: Restricts username length to between 3 and 30 characters
         * RegularExpression: 
         *   ^ : Start of string
         *   [a-zA-Z0-9_] : Matches any letter (lower or uppercase), number, or underscore
         *   + : One or more of the preceding character set
         *   $ : End of string
         */
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username must contain only letters, numbers, and underscores. No spaces or special characters are allowed.")]
        public string Username { get; set; }

        /*
         * Password property
         * Required: Ensures the field is not null or empty
         * StringLength: Restricts password length to between 8 and 128 characters
         * RegularExpression:
         *   ^ : Start of string
         *   (?=.*[a-z]) : Positive lookahead for at least one lowercase letter
         *   (?=.*[A-Z]) : Positive lookahead for at least one uppercase letter
         *   (?=.*\d) : Positive lookahead for at least one digit
         *   (?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]) : Positive lookahead for at least one special character
         *   .{8,} : Any character (except newline) at least 8 times
         *   $ : End of string
         */
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        /*
         * Password confirmation property
         * Required: Ensures the field is not null or empty
         * Compare: Validates that this field matches the Password field
         */
        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /*
         * Email address property
         * Required: Ensures the field is not null or empty
         * StringLength: Restricts email length to between 5 and 320 characters
         * EmailAddress: Validates that the input is in a valid email format
         */
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(320, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 320 characters long.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
    }
}