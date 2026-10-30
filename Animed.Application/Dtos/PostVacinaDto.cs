using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostVacinaDto
    {
        [Required]
        public string NomeVacina { get; set; } = string.Empty;
        [Required]
        public DateTime DataAplicacao { get; set; }
        public DateTime? DataProximaDose { get; set; }
    }
}
