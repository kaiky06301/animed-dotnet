using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Animed.Domain.Entities
{
    [Table("tb_net_consulta")]
    public class ConsultaEntity
    {
        public const int PontosPorConsulta = 20;

        [Key]
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        [Required]
        [StringLength(200)]
        public string Motivo { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Prescricao { get; set; }

        public int PetId { get; set; }
        [JsonIgnore]
        public PetEntity? Pet { get; set; }
    }
}
