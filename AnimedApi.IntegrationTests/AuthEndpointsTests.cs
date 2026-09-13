using System.Net;
using System.Net.Http.Json;
using AnimedApi.Dtos;
using Xunit;

namespace AnimedApi.IntegrationTests;

/// <summary>Testes de integracao de autenticacao (login e protecao de endpoints).</summary>
[Collection("Integration")]
public class AuthEndpointsTests
{
    private readonly AnimedWebApplicationFactory _factory;

    public AuthEndpointsTests(AnimedWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_ComCredenciaisValidas_RetornaOkComToken()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage resp = await client.PostAsJsonAsync(
            "/api/auth/login", new { usuario = "teste", senha = "senha-teste" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        LoginResponse? body = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_RetornaUnauthorized()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage resp = await client.PostAsJsonAsync(
            "/api/auth/login", new { usuario = "errado", senha = "errado" });

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task GetTutores_SemToken_RetornaUnauthorized()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage resp = await client.GetAsync("/api/tutores");

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
