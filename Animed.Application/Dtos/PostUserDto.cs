using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostUserDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>tutor, veterinario ou admin</summary>
        [Required]
        [RegularExpression("tutor|veterinario|admin", ErrorMessage = "Role deve ser tutor, veterinario ou admin")]
        public string Role { get; set; } = "tutor";
    }
}
