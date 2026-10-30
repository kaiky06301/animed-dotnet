using Animed.Domain.Entities;

namespace Animed.Tests.Unit.Domain
{
    public class TutorEntityTest
    {
        [Fact]
        [Trait("Domain", "Tutor")]
        public void AdicionarPontos_PontosValidos_SomaNoTotal()
        {
            // Arrange
            var tutor = new TutorEntity { PontosTotais = 50 };

            // Act
            tutor.AdicionarPontos(30);

            // Assert
            Assert.Equal(80, tutor.PontosTotais);
            Assert.Equal(NivelTutor.Basico, tutor.Nivel);
        }

        [Fact]
        [Trait("Domain", "Tutor")]
        public void AdicionarPontos_AtingeTrezentos_SobeParaCuidador()
        {
            // Arrange
            var tutor = new TutorEntity { PontosTotais = 290 };

            // Act
            tutor.AdicionarPontos(10);

            // Assert
            Assert.Equal(300, tutor.PontosTotais);
            Assert.Equal(NivelTutor.Cuidador, tutor.Nivel);
        }

        [Theory]
        [Trait("Domain", "Tutor")]
        [InlineData(0)]
        [InlineData(-5)]
        public void AdicionarPontos_PontosZeroOuNegativos_LancaArgumentException(int pontos)
        {
            // Arrange
            var tutor = new TutorEntity { PontosTotais = 100 };

            // Act
            var acao = () => tutor.AdicionarPontos(pontos);

            // Assert
            Assert.Throws<ArgumentException>(acao);
            Assert.Equal(100, tutor.PontosTotais);
        }

        [Theory]
        [Trait("Domain", "Tutor")]
        [InlineData(0, NivelTutor.Basico)]
        [InlineData(299, NivelTutor.Basico)]
        [InlineData(300, NivelTutor.Cuidador)]
        [InlineData(699, NivelTutor.Cuidador)]
        [InlineData(700, NivelTutor.Premium)]
        public void Calcular_FaixaDePontos_RetornaNivelCorreto(int pontos, string nivelEsperado)
        {
            // Arrange / Act
            var nivel = NivelTutor.Calcular(pontos);

            // Assert
            Assert.Equal(nivelEsperado, nivel);
        }
    }
}
