using System.ComponentModel.DataAnnotations;

namespace Restaurant8.Dtos.Account
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string NewRole { get; set; } = string.Empty;
    }
}
