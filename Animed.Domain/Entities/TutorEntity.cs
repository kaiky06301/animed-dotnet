using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Animed.Domain.Entities
{
    [Table("tb_net_tutor")]
    [Index(nameof(Email), IsUnique = true, Name = "IDX_NET_TUTOR_EMAIL")]
    public class TutorEntity
    {
        public const int PontosParaCuidador = 300;
        public const int PontosParaPremium = 700;

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [StringLength(14)]
        public string Cpf { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
        public int PontosTotais { get; set; }
        public string Nivel { get; set; } = NivelTutor.Basico;

        //1:N
        public ICollection<PetEntity>? Pets { get; set; } = [];

        /// <summary>
        /// Soma os pontos de uma ação de cuidado e recalcula o nível do tutor.
        /// </summary>
        public void AdicionarPontos(int pontos)
        {
            if (pontos <= 0) throw new ArgumentException("Os pontos devem ser maiores que zero");

            PontosTotais += pontos;
            Nivel = NivelTutor.Calcular(PontosTotais);
        }
    }

    public static class NivelTutor
    {
        public const string Basico = "BASICO";
        public const string Cuidador = "CUIDADOR";
        public const string Premium = "TUTOR_PREMIUM";

        public static string Calcular(int pontos)
        {
            if (pontos >= TutorEntity.PontosParaPremium) return Premium;
            if (pontos >= TutorEntity.PontosParaCuidador) return Cuidador;
            return Basico;
        }
    }
}
