using Azure.Monitor.OpenTelemetry.AspNetCore;
using Serilog;
using Serilog.Events;

namespace Animed.API.Config
{
    public static class ConfigurationAPI
    {
        /// <summary>
        /// Tracing e métricas no Application Insights. Só liga quando a
        /// connection string está configurada (local roda sem telemetria).
        /// </summary>
        public static void AddTelemetria(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ApplicationInsights:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString)) return;

            services.AddOpenTelemetry()
                .UseAzureMonitor(options =>
                {
                    options.ConnectionString = connectionString;
                });
        }

        public static void AddLogApi(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(AppContext.BaseDirectory, "logs", "api-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {RequestId} {RequestPath}{NewLine}    {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            services.AddSerilog();
        }
    }
}
