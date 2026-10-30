using Animed.Application.Dtos;
using Animed.Application.UseCases;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Animed.Tests.Unit.Application
{
    public class PackageAnimedUseCaseTest
    {
        private readonly Mock<IPackageAnimedRepository> _packageRepository = new();
        private readonly PackageAnimedUseCase _useCase;

        public PackageAnimedUseCaseTest()
        {
            _useCase = new PackageAnimedUseCase(_packageRepository.Object, new Mock<ILogger<PackageAnimedUseCase>>().Object);
        }

        [Fact]
        [Trait("UseCase", "Package")]
        public void RegistrarVacina_ProcedureExecuta_Retorna201ComPontos()
        {
            // Arrange
            _packageRepository
                .Setup(x => x.RegistrarVacina(1, "V10", It.IsAny<DateTime>(), null, null))
                .Returns(new VacinaPackageModel { IdVacina = 9, PontosCreditados = 30 });

            // Act
            var resultado = _useCase.RegistrarVacina(new PostVacinaPackageDto { PetId = 1, NomeVacina = "V10", DataAplicacao = DateTime.Today });

            // Assert
            Assert.Equal(201, resultado.StatusCode);
            Assert.Equal(30, resultado.Value?.PontosCreditados);
        }

        [Fact]
        [Trait("UseCase", "Package")]
        public void RegistrarVacina_PackageLancaPetInexistente_Retorna404()
        {
            // Arrange
            _packageRepository
                .Setup(x => x.RegistrarVacina(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<string?>()))
                .Throws(new NoContentException("Pet 999 nao encontrado."));

            // Act
            var resultado = _useCase.RegistrarVacina(new PostVacinaPackageDto { PetId = 999, NomeVacina = "V10", DataAplicacao = DateTime.Today });

            // Assert
            Assert.Equal(404, resultado.StatusCode);
            Assert.Equal("Pet 999 nao encontrado.", resultado.Error);
        }

        [Fact]
        [Trait("UseCase", "Package")]
        public void CalcularDesconto_Cuidador_RetornaValorEDesconto()
        {
            // Arrange
            _packageRepository.Setup(x => x.CalcularDesconto("CUIDADOR", 200, 0)).Returns(180);

            // Act
            var resultado = _useCase.CalcularDesconto("CUIDADOR", 200, 0);

            // Assert
            Assert.Equal(200, resultado.StatusCode);
            Assert.Equal(180, resultado.Value?.ValorFinal);
            Assert.Equal(20, resultado.Value?.Desconto);
        }
    }
}
