using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestSharp;

namespace Animed.Infrastructure.HealthChecks
{
    /// <summary>Confere se o serviço externo ViaCEP está no ar.</summary>
    public class ViaCepHealthCheck : IHealthCheck
    {
        private readonly RestClient _restClient = new("https://viacep.com.br/ws/");

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var response = await _restClient.ExecuteAsync(new RestRequest("01001000/json/", Method.Get), cancellationToken);

            return response.IsSuccessful
                ? HealthCheckResult.Healthy("ViaCEP respondendo")
                : HealthCheckResult.Degraded($"ViaCEP respondeu {(int)response.StatusCode}");
        }
    }
}
