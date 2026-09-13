using System.Net;
using System.Net.Http.Json;
using AnimedApi.Dtos;
using Xunit;

namespace AnimedApi.IntegrationTests;

/// <summary>Testes de integracao dos endpoints de Tutor (com autenticacao).</summary>
[Collection("Integration")]
public class TutoresEndpointsTests
{
    private readonly AnimedWebApplicationFactory _factory;

    public TutoresEndpointsTests(AnimedWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task PostTutor_ComTokenEDadosValidos_RetornaCreated()
    {
        // Arrange
        HttpClient client = await AutenticacaoHelper.ClienteAutenticadoAsync(_factory);

        // Act
        HttpResponseMessage resp = await client.PostAsJsonAsync(
            "/api/tutores", new { nome = "Ana", email = "ana@exemplo.com", cpf = "12345678901" });

        // Assert
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }

    [Fact]
    public async Task GetTutorPorId_QuandoNaoExiste_RetornaNotFound()
    {
        HttpClient client = await AutenticacaoHelper.ClienteAutenticadoAsync(_factory);

        HttpResponseMessage resp = await client.GetAsync("/api/tutores/999999");

        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task PostTutor_ComEmailInvalido_RetornaBadRequest()
    {
        HttpClient client = await AutenticacaoHelper.ClienteAutenticadoAsync(_factory);

        HttpResponseMessage resp = await client.PostAsJsonAsync(
            "/api/tutores", new { nome = "Ana", email = "nao-e-email", cpf = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }
}
