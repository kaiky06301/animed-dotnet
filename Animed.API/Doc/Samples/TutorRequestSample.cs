using Animed.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Animed.API.Doc.Samples
{
    public class TutorRequestSample : IExamplesProvider<PostTutorDto>
    {
        public PostTutorDto GetExamples()
        {
            return new PostTutorDto
            {
                Nome = "Marina Oliveira",
                Email = "marina.oliveira@email.com",
                Cpf = "123.456.789-00",
                Telefone = "(11) 98765-4321",
                Cep = "01310100"
            };
        }
    }
}
