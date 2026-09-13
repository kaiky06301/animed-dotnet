using System.Net;
using System.Net.Http.Json;
using AnimedApi.Dtos;
using Xunit;

namespace AnimedApi.IntegrationTests;

/// <summary>Testes de integracao dos endpoints de Pet, incluindo a checagem de FK (tutor) -> 400.</summary>
[Collection("Integration")]
public class PetsEndpointsTests
{
    private readonly AnimedWebApplicationFactory _factory;

    public PetsEndpointsTests(AnimedWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task PostPet_ComTutorInexistente_RetornaBadRequest()
    {
        // Arrange
        HttpClient client = await AutenticacaoHelper.ClienteAutenticadoAsync(_factory);

        // Act
        HttpResponseMessage resp = await client.PostAsJsonAsync(
            "/api/pets", new { nome = "Rex", especie = "CACHORRO", tutorId = 999999 });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task PostPet_ComTutorExistente_RetornaCreated()
    {
        // Arrange
        HttpClient client = await AutenticacaoHelper.ClienteAutenticadoAsync(_factory);
        HttpResponseMessage tutorResp = await client.PostAsJsonAsync(
            "/api/tutores", new { nome = "Dona Maria", email = "maria@exemplo.com", cpf = "11122233344" });
        TutorResponse? tutor = await tutorResp.Content.ReadFromJsonAsync<TutorResponse>();

        // Act
        HttpResponseMessage resp = await client.PostAsJsonAsync(
            "/api/pets", new { nome = "Rex", especie = "CACHORRO", tutorId = tutor!.Id });

        // Assert
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }
}
