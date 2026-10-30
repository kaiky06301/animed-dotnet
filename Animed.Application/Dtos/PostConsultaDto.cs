using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostConsultaDto
    {
        [Required]
        public DateTime DataHora { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Motivo { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Prescricao { get; set; }
    }
}
