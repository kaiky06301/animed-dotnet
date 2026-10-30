using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostTutorDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"\d{3}\.\d{3}\.\d{3}-\d{2}", ErrorMessage = "CPF no formato 000.000.000-00")]
        public string Cpf { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
    }
}
