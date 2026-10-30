using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Animed.Domain.Entities
{
    [Table("tb_net_pet")]
    [Index(nameof(TutorId), nameof(Nome))] //Indice Composto
    public class PetEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>CACHORRO ou GATO</summary>
        [Required]
        public string Especie { get; set; } = string.Empty;
        public string? Raca { get; set; }
        public DateTime? DataNascimento { get; set; }
        public double? PesoKg { get; set; }
        public bool Castrado { get; set; }

        public int TutorId { get; set; }
        public TutorEntity? Tutor { get; set; }

        //1:N
        public ICollection<VacinaEntity>? Vacinas { get; set; } = [];

        //1:N
        public ICollection<ConsultaEntity>? Consultas { get; set; } = [];
    }
}
