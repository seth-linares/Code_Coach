using System.ComponentModel.DataAnnotations;

namespace SeniorProjBackend.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Username is Required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
    }
}
