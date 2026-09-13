using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnimedApi.Dtos;

namespace AnimedApi.IntegrationTests;

/// <summary>Utilitario: faz login com o usuario de teste e devolve um HttpClient ja com o Bearer token.</summary>
public static class AutenticacaoHelper
{
    public static async Task<HttpClient> ClienteAutenticadoAsync(AnimedWebApplicationFactory factory)
    {
        HttpClient client = factory.CreateClient();
        HttpResponseMessage login = await client.PostAsJsonAsync(
            "/api/auth/login", new { usuario = "teste", senha = "senha-teste" });
        login.EnsureSuccessStatusCode();

        LoginResponse? body = await login.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
        return client;
    }
}
