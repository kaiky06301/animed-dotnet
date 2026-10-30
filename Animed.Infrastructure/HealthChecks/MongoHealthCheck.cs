using Animed.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Animed.Infrastructure.HealthChecks
{
    /// <summary>Confere se o MongoDB responde a uma consulta simples.</summary>
    public class MongoHealthCheck : IHealthCheck
    {
        private readonly MongoApplicationContext _context;

        public MongoHealthCheck(MongoApplicationContext context)
        {
            _context = context;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var total = _context.HistoricoCuidado.Count();
                return Task.FromResult(HealthCheckResult.Healthy($"MongoDB respondendo ({total} históricos)"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("MongoDB indisponível", ex));
            }
        }
    }
}
