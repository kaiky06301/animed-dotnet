using Animed.Application.Dtos;
using Animed.Domain.Entities;
using Animed.Tests.Integration.Fixtures;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Animed.Tests.Integration.Controllers
{
    [Collection("Api")]
    public class TutorControllerTest
    {
        private readonly AnimedApiFactory _factory;

        public TutorControllerTest(AnimedApiFactory factory)
        {
            _factory = factory;
        }

        private static OperationResult<PageResponseDto<GetTutorDto>> PaginaComUmTutor()
        {
            return OperationResult<PageResponseDto<GetTutorDto>>.Success(new PageResponseDto<GetTutorDto>
            {
                Items = [new GetTutorDto { Id = 1, Nome = "Marina", Links = [new LinkDto("/api/tutor/1", "get_um_tutor")] }],
                TotalRegistros = 1,
                Links = [new LinkDto("/api/tutor?Deslocamento=0&RegistroRetornado=10", "self")]
            });
        }

        [Fact]
        [Trait("Integration", "Tutor")]
        public async Task Get_SemToken_Retorna401()
        {
            // Arrange
            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/tutor");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        [Trait("Integration", "Tutor")]
        public async Task Get_RoleTutor_Retorna403()
        {
            // Arrange
            using var client = _factory.ClienteComRole("tutor");

            // Act
            var response = await client.GetAsync("/api/tutor");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        [Trait("Integration", "Tutor")]
        public async Task Get_RoleVeterinario_Retorna200ComHateoas()
        {
            // Arrange
            _factory.TutorUseCaseMock
                .Setup(x => x.ObterTodosTutores(It.IsAny<FiltroDto>(), It.IsAny<string?>()))
                .Returns(PaginaComUmTutor());
            using var client = _factory.ClienteComRole("veterinario");

            // Act
            var response = await client.GetAsync("/api/tutor?Deslocamento=0&RegistroRetornado=10&OrdenarPor=nome");
            var pagina = await response.Content.ReadFromJsonAsync<PageResponseDto<GetTutorDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Marina", pagina!.Items.Single().Nome);
            Assert.Contains(pagina.Links, l => l.Rel == "self");
            _factory.TutorUseCaseMock.Verify(x => x.ObterTodosTutores(It.Is<FiltroDto>(f => f.OrdenarPor == "nome" && f.RegistroRetornado == 10), null), Times.AtLeastOnce);
        }

        [Fact]
        [Trait("Integration", "Tutor")]
        public async Task GetPorId_TutorInexistente_Retorna404ComMensagem()
        {
            // Arrange
            _factory.TutorUseCaseMock
                .Setup(x => x.ObterUmTutor(404))
                .Returns(OperationResult<GetTutorDto?>.Failure("Não foi encontrado o tutor", 404));
            using var client = _factory.ClienteComRole("tutor");

            // Act
            var response = await client.GetAsync("/api/tutor/404");
            var corpo = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Não foi encontrado o tutor", corpo);
        }

        [Fact]
        [Trait("Integration", "Tutor")]
        public async Task Post_CorpoInvalido_Retorna400SemChamarUseCase()
        {
            // Arrange
            using var client = _factory.ClienteComRole("admin");
            var invalido = new { Nome = "A", Email = "nao-e-email", Cpf = "123" };

            // Act
            var response = await client.PostAsJsonAsync("/api/tutor", invalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            _factory.TutorUseCaseMock.Verify(x => x.AdicionarTutor(It.Is<PostTutorDto>(d => d.Email == "nao-e-email")), Times.Never);
        }

        [Fact]
        [Trait("Integration", "Tutor")]
        public async Task Delete_RoleVeterinario_Retorna403()
        {
            // Arrange
            using var client = _factory.ClienteComRole("veterinario");

            // Act
            var response = await client.DeleteAsync("/api/tutor/1");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
