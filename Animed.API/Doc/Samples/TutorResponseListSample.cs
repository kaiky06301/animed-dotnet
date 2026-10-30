using Animed.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Animed.API.Doc.Samples
{
    public class TutorResponseListSample : IExamplesProvider<PageResponseDto<GetTutorDto>>
    {
        public PageResponseDto<GetTutorDto> GetExamples()
        {
            return new PageResponseDto<GetTutorDto>
            {
                Items = new List<GetTutorDto>
                {
                    new GetTutorDto
                    {
                        Id = 1,
                        Nome = "Marina Oliveira",
                        Email = "marina.oliveira@email.com",
                        Cpf = "123.456.789-00",
                        PontosTotais = 350,
                        Nivel = "CUIDADOR",
                        QuantidadePets = 2,
                        Links = new List<LinkDto>
                        {
                            new LinkDto("/api/tutor/1", "get_um_tutor", "GET"),
                            new LinkDto("/api/tutor/1", "update_tutor", "PUT"),
                            new LinkDto("/api/tutor/1", "delete_tutor", "DELETE"),
                            new LinkDto("/api/pet?TutorId=1", "pets_do_tutor", "GET"),
                        }
                    }
                },
                Deslocamento = 0,
                RegistroRetornado = 10,
                TotalRegistros = 1,
                Links = new List<LinkDto>
                {
                    new LinkDto("/api/tutor?Deslocamento=0&RegistroRetornado=10", "self", "GET"),
                    new LinkDto("/api/tutor", "create_tutor", "POST")
                }
            };
        }
    }
}
