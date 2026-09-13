using AnimedApi.Models;
using Xunit;

namespace AnimedApi.UnitTests.Domain;

/// <summary>
/// Regras do próprio tutor, testadas sem banco nem mock.
///
/// A validação mora na entidade: um tutor sem nome ou sem e-mail não chega a
/// existir, em vez de ser criado quebrado e recusado mais adiante.
/// </summary>
public class TutorTests
{
    [Fact]
    public void Criar_ComDadosValidos_CriaTutor()
    {
        // Arrange
        const string nome = "Ana";
        const string email = "ana@exemplo.com";
        const string cpf = "12345678901";

        // Act
        Tutor tutor = Tutor.Criar(nome, email, cpf, null);

        // Assert
        Assert.Equal(nome, tutor.Nome);
        Assert.Equal(email, tutor.Email);
        Assert.Equal(cpf, tutor.Cpf);
    }

    [Fact]
    public void Criar_ComNomeVazio_LancaDomainException()
    {
        // Arrange
        const string nomeVazio = "";

        // Act
        var criar = () => Tutor.Criar(nomeVazio, "ana@exemplo.com", "12345678901", null);

        // Assert
        Assert.Throws<DomainException>(criar);
    }

    [Fact]
    public void Criar_ComEmailVazio_LancaDomainException()
    {
        // Arrange: só espaços também conta como ausente
        const string emailEmBranco = "  ";

        // Act
        var criar = () => Tutor.Criar("Ana", emailEmBranco, "12345678901", null);

        // Assert
        Assert.Throws<DomainException>(criar);
    }
}
