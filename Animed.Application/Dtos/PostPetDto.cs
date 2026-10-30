using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostPetDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;
        [Required]
        [RegularExpression("CACHORRO|GATO", ErrorMessage = "Especie deve ser CACHORRO ou GATO")]
        public string Especie { get; set; } = string.Empty;
        public string? Raca { get; set; }
        public DateTime? DataNascimento { get; set; }
        [Range(0.1, 120)]
        public double? PesoKg { get; set; }
        public bool Castrado { get; set; }
        [Required]
        public int TutorId { get; set; }
    }
}
