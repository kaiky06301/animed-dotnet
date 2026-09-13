using System;
using AnimedApi.Models;
using Xunit;

namespace AnimedApi.UnitTests.Domain;

public class VacinaTests
{
    [Fact]
    public void Criar_ComDataPassada_CriaVacina()
    {
        // Arrange
        var ontem = DateTime.UtcNow.AddDays(-1);

        // Act
        var vacina = Vacina.Criar("Antirrabica", ontem, null, petId: 1);

        // Assert
        Assert.Equal("Antirrabica", vacina.Nome);
    }

    [Fact]
    public void Criar_ComDataFutura_LancaDomainException()
    {
        // Arrange
        var amanha = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Vacina.Criar("Antirrabica", amanha, null, petId: 1));
        Assert.Contains("futura", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Criar_ComNomeVazio_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Vacina.Criar("", DateTime.UtcNow.AddDays(-1), null, petId: 1));
    }
}
