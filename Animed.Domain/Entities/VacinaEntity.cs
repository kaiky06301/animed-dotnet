using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Animed.Domain.Entities
{
    [Table("tb_net_vacina")]
    public class VacinaEntity
    {
        public const int PontosPorVacina = 30;

        [Key]
        public int Id { get; set; }
        [Required]
        public string NomeVacina { get; set; } = string.Empty;
        public DateTime DataAplicacao { get; set; }
        public DateTime? DataProximaDose { get; set; }

        public int PetId { get; set; }
        [JsonIgnore]
        public PetEntity? Pet { get; set; }

        public bool Vencida(DateTime hoje) => DataProximaDose.HasValue && DataProximaDose.Value.Date < hoje.Date;
    }
}
