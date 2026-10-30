using Animed.Application.Dtos;
using Animed.Application.UseCases;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Animed.Tests.Unit.Application
{
    public class PetUseCaseTest
    {
        private readonly Mock<IPetRepository> _petRepository = new();
        private readonly Mock<ITutorRepository> _tutorRepository = new();
        private readonly Mock<IHistoricoCuidadoRepository> _historicoRepository = new();
        private readonly Mock<ILogger<PetUseCase>> _logger = new();
        private readonly PetUseCase _petUseCase;

        private readonly PetEntity _thor = new() { Id = 7, Nome = "Thor", Especie = "CACHORRO", TutorId = 3 };

        public PetUseCaseTest()
        {
            _petUseCase = new PetUseCase(_petRepository.Object, _tutorRepository.Object, _historicoRepository.Object, _logger.Object);
        }

        [Fact]
        [Trait("UseCase", "Pet")]
        public void RegistrarVacina_DadosValidos_CreditaPontosEGravaHistorico()
        {
            // Arrange
            var dto = new PostVacinaDto { NomeVacina = "V10", DataAplicacao = DateTime.Today, DataProximaDose = DateTime.Today.AddYears(1) };
            _petRepository.Setup(x => x.ObterUm(7)).Returns(_thor);
            _petRepository.Setup(x => x.AdicionarVacina(It.IsAny<VacinaEntity>(), 7)).Returns(new VacinaEntity { Id = 1, NomeVacina = "V10", PetId = 7 });

            // Act
            var resultado = _petUseCase.RegistrarVacina(7, dto);

            // Assert
            Assert.Equal(201, resultado.StatusCode);
            Assert.Equal(VacinaEntity.PontosPorVacina, resultado.Value?.PontosGanhos);
            _tutorRepository.Verify(x => x.AdicionarPontos(3, VacinaEntity.PontosPorVacina), Times.Once);
            _historicoRepository.Verify(x => x.RegistrarEvento(7, 3, "Thor", It.Is<EventoCuidadoEntity>(e => e.Tipo == "VACINA")), Times.Once);
        }

        [Fact]
        [Trait("UseCase", "Pet")]
        public void RegistrarVacina_DataNoFuturo_Retorna400SemSalvar()
        {
            // Arrange
            var dto = new PostVacinaDto { NomeVacina = "V10", DataAplicacao = DateTime.Today.AddDays(3) };

            // Act
            var resultado = _petUseCase.RegistrarVacina(7, dto);

            // Assert
            Assert.Equal(400, resultado.StatusCode);
            Assert.Equal("A data de aplicação não pode estar no futuro", resultado.Error);
            _petRepository.Verify(x => x.AdicionarVacina(It.IsAny<VacinaEntity>(), It.IsAny<int>()), Times.Never);
            _tutorRepository.Verify(x => x.AdicionarPontos(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        [Trait("UseCase", "Pet")]
        public void RegistrarVacina_PetNaoExiste_Retorna404()
        {
            // Arrange
            var dto = new PostVacinaDto { NomeVacina = "V10", DataAplicacao = DateTime.Today };
            _petRepository.Setup(x => x.ObterUm(99)).Throws(new NoContentException("Não foi encontrado o pet"));

            // Act
            var resultado = _petUseCase.RegistrarVacina(99, dto);

            // Assert
            Assert.Equal(404, resultado.StatusCode);
        }

        [Fact]
        [Trait("UseCase", "Pet")]
        public void RegistrarVacina_MongoIndisponivel_AindaRetorna201()
        {
            // Arrange
            var dto = new PostVacinaDto { NomeVacina = "Antirrábica", DataAplicacao = DateTime.Today };
            _petRepository.Setup(x => x.ObterUm(7)).Returns(_thor);
            _petRepository.Setup(x => x.AdicionarVacina(It.IsAny<VacinaEntity>(), 7)).Returns(new VacinaEntity { Id = 2, NomeVacina = "Antirrábica", PetId = 7 });
            _historicoRepository
                .Setup(x => x.RegistrarEvento(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EventoCuidadoEntity>()))
                .Throws(new Exception("Mongo fora do ar"));

            // Act
            var resultado = _petUseCase.RegistrarVacina(7, dto);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(201, resultado.StatusCode);
        }

        [Fact]
        [Trait("UseCase", "Pet")]
        public void ObterUmPet_PetExiste_RetornaLinkDoHistorico()
        {
            // Arrange
            _petRepository.Setup(x => x.ObterUm(7)).Returns(_thor);

            // Act
            var resultado = _petUseCase.ObterUmPet(7);

            // Assert
            Assert.Equal(200, resultado.StatusCode);
            Assert.Contains(resultado.Value!.Links, l => l.Rel == "historico_de_cuidados" && l.Href == "/api/historico/pet/7");
            Assert.Contains(resultado.Value.Links, l => l.Rel == "tutor" && l.Href == "/api/tutor/3");
        }
    }
}
