using Animed.Domain.Entities;

namespace Animed.Tests.Unit.Domain
{
    public class VacinaEntityTest
    {
        private static readonly DateTime Hoje = new(2026, 11, 4);

        [Fact]
        [Trait("Domain", "Vacina")]
        public void Vencida_ProximaDoseNoPassado_RetornaTrue()
        {
            // Arrange
            var vacina = new VacinaEntity { DataProximaDose = Hoje.AddDays(-1) };

            // Act
            var vencida = vacina.Vencida(Hoje);

            // Assert
            Assert.True(vencida);
        }

        [Fact]
        [Trait("Domain", "Vacina")]
        public void Vencida_ProximaDoseHoje_RetornaFalse()
        {
            // Arrange
            var vacina = new VacinaEntity { DataProximaDose = Hoje };

            // Act / Assert
            Assert.False(vacina.Vencida(Hoje));
        }

        [Fact]
        [Trait("Domain", "Vacina")]
        public void Vencida_SemProximaDose_RetornaFalse()
        {
            // Arrange
            var vacina = new VacinaEntity { DataProximaDose = null };

            // Act / Assert
            Assert.False(vacina.Vencida(Hoje));
        }
    }
}
