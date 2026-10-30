using Animed.Application.Dtos;
using Animed.Application.UseCases;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Animed.Tests.Unit.Application
{
    public class TutorUseCaseTest
    {
        private readonly Mock<ITutorRepository> _tutorRepository = new();
        private readonly Mock<IEnderecoService> _enderecoService = new();
        private readonly Mock<ILogger<TutorUseCase>> _logger = new();
        private readonly TutorUseCase _tutorUseCase;

        public TutorUseCaseTest()
        {
            _tutorUseCase = new TutorUseCase(_tutorRepository.Object, _enderecoService.Object, _logger.Object);
        }

        [Fact]
        [Trait("UseCase", "Tutor")]
        public void ObterUmTutor_TutorExiste_RetornaDtoComLinksHateoas()
        {
            // Arrange
            var tutor = new TutorEntity { Id = 1, Nome = "Marina", Email = "marina@email.com", PontosTotais = 350, Nivel = NivelTutor.Cuidador };
            _tutorRepository.Setup(x => x.ObterUm(1)).Returns(tutor);

            // Act
            var resultado = _tutorUseCase.ObterUmTutor(1);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(200, resultado.StatusCode);
            Assert.Equal("Marina", resultado.Value?.Nome);
            Assert.Contains(resultado.Value!.Links, l => l.Rel == "pets_do_tutor" && l.Href == "/api/pet?TutorId=1");
        }

        [Fact]
        [Trait("UseCase", "Tutor")]
        public void ObterUmTutor_TutorNaoExiste_Retorna404()
        {
            // Arrange
            _tutorRepository.Setup(x => x.ObterUm(99)).Throws(new NoContentException("Não foi encontrado o tutor"));

            // Act
            var resultado = _tutorUseCase.ObterUmTutor(99);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(404, resultado.StatusCode);
            Assert.Equal("Não foi encontrado o tutor", resultado.Error);
        }

        [Fact]
        [Trait("UseCase", "Tutor")]
        public void AdicionarTutor_EmailDuplicado_Retorna409()
        {
            // Arrange
            var dto = new PostTutorDto { Nome = "Marina", Email = "marina@email.com", Cpf = "123.456.789-00" };
            _tutorRepository.Setup(x => x.Adicionar(It.IsAny<TutorEntity>())).Throws(new RegraDeNegocioException("Já existe um tutor com este e-mail"));

            // Act
            var resultado = _tutorUseCase.AdicionarTutor(dto);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(409, resultado.StatusCode);
        }

        [Fact]
        [Trait("UseCase", "Tutor")]
        public void ObterTodosTutores_PrimeiraPaginaComMais_RetornaLinkNextPage()
        {
            // Arrange
            var tutores = new List<TutorEntity>
            {
                new() { Id = 1, Nome = "Ana" },
                new() { Id = 2, Nome = "Bruno" },
            };
            _tutorRepository
                .Setup(x => x.ObterTodos(0, 2, null, null, null, false))
                .Returns((tutores, 5));

            // Act
            var resultado = _tutorUseCase.ObterTodosTutores(new FiltroDto { Deslocamento = 0, RegistroRetornado = 2 }, null);

            // Assert
            Assert.Equal(200, resultado.StatusCode);
            Assert.Equal(5, resultado.Value!.TotalRegistros);
            Assert.Equal(2, resultado.Value.Items.Count());
            Assert.Contains(resultado.Value.Links, l => l.Rel == "next_page" && l.Href.Contains("Deslocamento=2"));
            Assert.DoesNotContain(resultado.Value.Links, l => l.Rel == "previous_page");
        }

        [Fact]
        [Trait("UseCase", "Tutor")]
        public void ObterTodosTutores_SemResultados_Retorna204()
        {
            // Arrange
            _tutorRepository
                .Setup(x => x.ObterTodos(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .Throws(new NoContentException("Não foi encontrado tutores"));

            // Act
            var resultado = _tutorUseCase.ObterTodosTutores(new FiltroDto(), null);

            // Assert
            Assert.Equal(204, resultado.StatusCode);
        }

        [Fact]
        [Trait("UseCase", "Tutor")]
        public async Task ObterEnderecoPorCepAsync_CepInexistente_Retorna404()
        {
            // Arrange
            _enderecoService.Setup(x => x.ObterEnderecoPorCepAsync("00000000")).ThrowsAsync(new NoContentException("Não foi localizado"));

            // Act
            var resultado = await _tutorUseCase.ObterEnderecoPorCepAsync("00000000");

            // Assert
            Assert.Equal(404, resultado.StatusCode);
        }
    }
}
