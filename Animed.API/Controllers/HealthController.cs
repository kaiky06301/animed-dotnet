using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace Animed.API.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("live")]
        [SwaggerOperation(Summary = "Liveness: a API está de pé")]
        public Task<IActionResult> Live(CancellationToken ct) => Relatorio("live", ct);

        [HttpGet("db")]
        [SwaggerOperation(Summary = "Readiness: Oracle e MongoDB")]
        public Task<IActionResult> DbReady(CancellationToken ct) => Relatorio("db", ct);

        [HttpGet("externo")]
        [SwaggerOperation(Summary = "Serviços externos: ViaCEP")]
        public Task<IActionResult> Externo(CancellationToken ct) => Relatorio("externo", ct);

        private async Task<IActionResult> Relatorio(string tag, CancellationToken ct)
        {
            var report = await _healthCheckService.CheckHealthAsync(r => r.Tags.Contains(tag), ct);

            var result = new
            {
                status = report.Status.ToString(),
                duracaoMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(e => new {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    error = e.Value.Exception?.Message
                })
            };

            return report.Status == HealthStatus.Unhealthy
                ? StatusCode(StatusCodes.Status503ServiceUnavailable, result)
                : Ok(result);
        }
    }
}
