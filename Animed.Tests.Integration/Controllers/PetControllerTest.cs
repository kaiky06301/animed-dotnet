using Animed.Application.Dtos;
using Animed.Domain.Entities;
using Animed.Tests.Integration.Fixtures;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Animed.Tests.Integration.Controllers
{
    [Collection("Api")]
    public class PetControllerTest
    {
        private readonly AnimedApiFactory _factory;

        public PetControllerTest(AnimedApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        [Trait("Integration", "Pet")]
        public async Task PostVacina_RoleTutor_Retorna403()
        {
            // Arrange
            using var client = _factory.ClienteComRole("tutor");
            var vacina = new PostVacinaDto { NomeVacina = "V10", DataAplicacao = DateTime.Today };

            // Act
            var response = await client.PostAsJsonAsync("/api/pet/7/vacina", vacina);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        [Trait("Integration", "Pet")]
        public async Task PostVacina_RoleVeterinario_Retorna201ComPontos()
        {
            // Arrange
            _factory.PetUseCaseMock
                .Setup(x => x.RegistrarVacina(7, It.IsAny<PostVacinaDto>()))
                .Returns(OperationResult<GetVacinaDto?>.Success(new GetVacinaDto { Id = 1, NomeVacina = "V10", PontosGanhos = 30 }, 201));
            using var client = _factory.ClienteComRole("veterinario");

            // Act
            var response = await client.PostAsJsonAsync("/api/pet/7/vacina", new PostVacinaDto { NomeVacina = "V10", DataAplicacao = DateTime.Today });
            var vacina = await response.Content.ReadFromJsonAsync<GetVacinaDto>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(30, vacina!.PontosGanhos);
        }

        [Fact]
        [Trait("Integration", "Pet")]
        public async Task Get_SemResultados_Retorna204()
        {
            // Arrange
            _factory.PetUseCaseMock
                .Setup(x => x.ObterTodosPets(It.IsAny<FiltroDto>(), It.IsAny<int?>(), "GATO"))
                .Returns(OperationResult<PageResponseDto<GetPetDto>>.Failure("Não foi encontrado pets", 204));
            using var client = _factory.ClienteComRole("tutor");

            // Act
            var response = await client.GetAsync("/api/pet?Especie=GATO");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
