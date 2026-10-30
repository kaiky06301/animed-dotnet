using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostAuthDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Senha { get; set; } = string.Empty;
    }
}
