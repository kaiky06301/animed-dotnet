using System;
using AnimedApi.Models;
using Xunit;

namespace AnimedApi.UnitTests.Domain;

/// <summary>
/// Regras da própria consulta, testadas sem banco nem mock.
///
/// Motivo e data e hora são o que dá sentido ao registro clínico: sem eles a
/// consulta não é criada, em vez de virar uma linha vazia no histórico do pet.
/// </summary>
public class ConsultaTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaConsulta()
    {
        // Arrange
        DateTime quando = DateTime.UtcNow;
        const string motivo = "Check-up";

        // Act
        Consulta consulta = Consulta.Criar(quando, motivo, null, null, petId: 1);

        // Assert
        Assert.Equal(motivo, consulta.Motivo);
        Assert.Equal(quando, consulta.DataHora);
        Assert.Equal(1, consulta.PetId);
    }

    [Fact]
    public void Criar_ComMotivoVazio_LancaDomainException()
    {
        // Arrange: só espaços também conta como ausente
        const string motivoEmBranco = "  ";

        // Act
        var criar = () => Consulta.Criar(DateTime.UtcNow, motivoEmBranco, null, null, petId: 1);

        // Assert
        Assert.Throws<DomainException>(criar);
    }

    [Fact]
    public void Criar_ComDataHoraPadrao_LancaDomainException()
    {
        // Arrange: default(DateTime) representa data não informada
        DateTime dataNaoInformada = default;

        // Act
        var criar = () => Consulta.Criar(dataNaoInformada, "Check-up", null, null, petId: 1);

        // Assert
        Assert.Throws<DomainException>(criar);
    }
}
