using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Mappers
{
    public static class PetMapper
    {
        public static PetEntity ToPetEntity(this PostPetDto obj)
        {
            return new PetEntity
            {
                Nome = obj.Nome,
                Especie = obj.Especie.ToUpper(),
                Raca = obj.Raca,
                DataNascimento = obj.DataNascimento,
                PesoKg = obj.PesoKg,
                Castrado = obj.Castrado,
                TutorId = obj.TutorId
            };
        }

        public static GetPetDto ToPetDto(this PetEntity obj, IEnumerable<LinkDto> links)
        {
            return new GetPetDto
            {
                Id = obj.Id,
                Nome = obj.Nome,
                Especie = obj.Especie,
                Raca = obj.Raca,
                DataNascimento = obj.DataNascimento,
                PesoKg = obj.PesoKg,
                Castrado = obj.Castrado,
                TutorId = obj.TutorId,
                Vacinas = obj.Vacinas?.Select(x => x.ToVacinaDto()).ToList() ?? Enumerable.Empty<GetVacinaDto>().ToList(),
                Consultas = obj.Consultas?.OrderByDescending(x => x.DataHora).Select(x => x.ToConsultaDto()).ToList() ?? Enumerable.Empty<GetConsultaDto>().ToList(),
                Links = links?.ToList() ?? Enumerable.Empty<LinkDto>().ToList()
            };
        }

        public static VacinaEntity ToVacinaEntity(this PostVacinaDto obj)
        {
            return new VacinaEntity
            {
                NomeVacina = obj.NomeVacina,
                DataAplicacao = obj.DataAplicacao,
                DataProximaDose = obj.DataProximaDose
            };
        }

        public static ConsultaEntity ToConsultaEntity(this PostConsultaDto obj)
        {
            return new ConsultaEntity
            {
                DataHora = obj.DataHora,
                Motivo = obj.Motivo,
                Diagnostico = obj.Diagnostico,
                Prescricao = obj.Prescricao
            };
        }

        public static GetConsultaDto ToConsultaDto(this ConsultaEntity obj, int pontosGanhos = 0)
        {
            return new GetConsultaDto
            {
                Id = obj.Id,
                DataHora = obj.DataHora,
                Motivo = obj.Motivo,
                Diagnostico = obj.Diagnostico,
                Prescricao = obj.Prescricao,
                PontosGanhos = pontosGanhos
            };
        }

        public static GetVacinaDto ToVacinaDto(this VacinaEntity obj, int pontosGanhos = 0)
        {
            return new GetVacinaDto
            {
                Id = obj.Id,
                NomeVacina = obj.NomeVacina,
                DataAplicacao = obj.DataAplicacao,
                DataProximaDose = obj.DataProximaDose,
                Vencida = obj.Vencida(DateTime.Today),
                PontosGanhos = pontosGanhos
            };
        }
    }
}
