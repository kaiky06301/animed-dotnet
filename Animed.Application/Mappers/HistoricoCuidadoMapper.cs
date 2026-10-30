using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Mappers
{
    public static class HistoricoCuidadoMapper
    {
        public static EventoCuidadoEntity ToEventoEntity(this PostEventoCuidadoDto obj)
        {
            return new EventoCuidadoEntity
            {
                Tipo = obj.Tipo.ToUpper(),
                Descricao = obj.Descricao,
                Pontos = obj.Pontos,
                Data = DateTime.UtcNow
            };
        }

        public static GetHistoricoCuidadoDto ToHistoricoDto(this HistoricoCuidadoEntity obj, IEnumerable<LinkDto> links)
        {
            return new GetHistoricoCuidadoDto
            {
                Id = obj.Id.ToString(),
                PetId = obj.PetId,
                TutorId = obj.TutorId,
                NomePet = obj.NomePet,
                TotalPontos = obj.Eventos?.Sum(x => x.Pontos) ?? 0,
                Eventos = obj.Eventos?.OrderByDescending(x => x.Data).ToList() ?? [],
                Links = links?.ToList() ?? Enumerable.Empty<LinkDto>().ToList()
            };
        }
    }
}
