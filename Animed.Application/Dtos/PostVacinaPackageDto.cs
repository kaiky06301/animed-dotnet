using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostVacinaPackageDto
    {
        [Required]
        public int PetId { get; set; }
        [Required]
        public string NomeVacina { get; set; } = string.Empty;
        [Required]
        public DateTime DataAplicacao { get; set; }
        public DateTime? DataProximaDose { get; set; }
        public string? Veterinario { get; set; }
    }
}
