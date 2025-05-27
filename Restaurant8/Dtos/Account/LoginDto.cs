using System.ComponentModel.DataAnnotations;

namespace Restaurant8.Dtos.Account
{
    public class LoginDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
