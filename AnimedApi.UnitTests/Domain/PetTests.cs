using AnimedApi.Models;
using Xunit;

namespace AnimedApi.UnitTests.Domain;

public class PetTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaPet()
    {
        // Arrange & Act
        var pet = Pet.Criar("Rex", "CACHORRO", "SRD", 12.5m, tutorId: 1);

        // Assert
        Assert.Equal("Rex", pet.Nome);
        Assert.Equal(12.5m, pet.Peso);
    }

    [Fact]
    public void Criar_ComPesoZero_LancaDomainException()
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<DomainException>(() => Pet.Criar("Rex", "CACHORRO", null, 0m, tutorId: 1));
        Assert.Contains("peso", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Criar_SemPeso_CriaPet()
    {
        // peso e opcional: null deve ser aceito
        var pet = Pet.Criar("Mimi", "GATO", null, null, tutorId: 2);
        Assert.Null(pet.Peso);
    }

    [Fact]
    public void Criar_ComNomeVazio_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Pet.Criar("  ", "CACHORRO", null, 5m, tutorId: 1));
    }

    [Fact]
    public void Criar_ComTutorIdInvalido_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Pet.Criar("Rex", "CACHORRO", null, 5m, tutorId: 0));
    }

    [Fact]
    public void Atualizar_ComPesoNegativo_LancaDomainException()
    {
        // Arrange
        var pet = Pet.Criar("Rex", "CACHORRO", null, 5m, tutorId: 1);

        // Act & Assert
        Assert.Throws<DomainException>(() => pet.Atualizar("Rex", "CACHORRO", null, -1m, tutorId: 1));
    }
}
