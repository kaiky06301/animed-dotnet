using System.ComponentModel.DataAnnotations;

namespace Animed.Application.Dtos
{
    public class PostEventoCuidadoDto
    {
        [Required]
        [RegularExpression("VACINA|CONSULTA|PESAGEM|MEDICAMENTO", ErrorMessage = "Tipo deve ser VACINA, CONSULTA, PESAGEM ou MEDICAMENTO")]
        public string Tipo { get; set; } = string.Empty;
        [Required]
        public string Descricao { get; set; } = string.Empty;
        [Range(0, 100)]
        public int Pontos { get; set; }
    }
}
