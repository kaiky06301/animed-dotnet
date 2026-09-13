using AnimedApi.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AnimedApi.HealthChecks;

/// <summary>
/// Health check de conectividade com o Oracle. Usa o proprio DbContext para verificar se a
/// aplicacao consegue abrir conexao com o banco (Database.CanConnectAsync).
/// </summary>
public class OracleHealthCheck : IHealthCheck
{
    private readonly AnimedDbContext _context;

    public OracleHealthCheck(AnimedDbContext context) => _context = context;

    /// <summary>Retorna Healthy se conecta ao banco; Unhealthy caso contrario.</summary>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            bool conectou = await _context.Database.CanConnectAsync(cancellationToken);
            return conectou
                ? HealthCheckResult.Healthy("Conexao com o Oracle OK.")
                : HealthCheckResult.Unhealthy("Nao foi possivel conectar ao Oracle.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao conectar ao Oracle.", ex);
        }
    }
}
