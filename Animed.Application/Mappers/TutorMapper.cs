using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Mappers
{
    public static class TutorMapper
    {
        public static TutorEntity ToTutorEntity(this PostTutorDto obj)
        {
            return new TutorEntity
            {
                Nome = obj.Nome,
                Email = obj.Email,
                Cpf = obj.Cpf,
                Telefone = obj.Telefone,
                Cep = obj.Cep
            };
        }

        public static GetTutorDto ToTutorDto(this TutorEntity obj, IEnumerable<LinkDto> links)
        {
            return new GetTutorDto
            {
                Id = obj.Id,
                Nome = obj.Nome,
                Email = obj.Email,
                Cpf = obj.Cpf,
                Telefone = obj.Telefone,
                Cep = obj.Cep,
                PontosTotais = obj.PontosTotais,
                Nivel = obj.Nivel,
                QuantidadePets = obj.Pets?.Count ?? 0,
                Links = links?.ToList() ?? Enumerable.Empty<LinkDto>().ToList()
            };
        }
    }
}
