using Animed.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Animed.API.Doc.Samples
{
    public class PetRequestSample : IExamplesProvider<PostPetDto>
    {
        public PostPetDto GetExamples()
        {
            return new PostPetDto
            {
                Nome = "Thor",
                Especie = "CACHORRO",
                Raca = "Golden Retriever",
                DataNascimento = new DateTime(2021, 3, 15),
                PesoKg = 28,
                Castrado = true,
                TutorId = 1
            };
        }
    }
}
