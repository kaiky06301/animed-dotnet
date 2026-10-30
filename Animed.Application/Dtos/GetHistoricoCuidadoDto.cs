using Animed.Domain.Entities;

namespace Animed.Application.Dtos
{
    public class GetHistoricoCuidadoDto
    {
        public string Id { get; set; } = string.Empty;
        public int PetId { get; set; }
        public int TutorId { get; set; }
        public string NomePet { get; set; } = string.Empty;
        public int TotalPontos { get; set; }

        public IEnumerable<EventoCuidadoEntity> Eventos { get; set; } = [];

        public List<LinkDto> Links { get; set; } = new();
    }
}
