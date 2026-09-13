using AnimedApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AnimedApi.IntegrationTests;

/// <summary>
/// Sobe a API em memoria para os testes de integracao. Troca o Oracle por um SQLite in-memory
/// (roda em qualquer maquina, sem banco externo) e injeta config de JWT/credenciais de teste.
///
/// A config JWT vai por variavel de ambiente (definida no construtor, antes de o host existir),
/// e nao por ConfigureAppConfiguration. Motivo: o AddJwtBearer le a chave no startup; se o override
/// so chegasse depois (via ConfigureAppConfiguration), a validacao usaria uma chave diferente da que
/// o TokenService usa em runtime, e todo request autenticado falharia com 401. Variavel de ambiente
/// e lida pelo CreateBuilder logo no inicio, garantindo a mesma chave nos dois lados.
/// </summary>
public class AnimedWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public AnimedWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Key", "chave-de-teste-suficientemente-longa-para-hmac-sha256-0123456789");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "AnimedApi.Testes");
        Environment.SetEnvironmentVariable("Jwt__Audience", "AnimedApi.Testes");
        Environment.SetEnvironmentVariable("Auth__Usuario", "teste");
        Environment.SetEnvironmentVariable("Auth__Senha", "senha-teste");

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Environment proprio: evita carregar appsettings.Development.json.
        builder.UseEnvironment("Testing");

        // ConfigureTestServices roda DEPOIS do Program.cs, permitindo sobrescrever o DbContext.
        builder.ConfigureTestServices(services =>
        {
            // AddDbContext registra a lambda do provider (UseOracle) em IDbContextOptionsConfiguration<T>,
            // alem de DbContextOptions<T> e DbContextOptions. Removemos os tres para trocar por SQLite.
            Type[] tiposParaRemover =
            {
                typeof(IDbContextOptionsConfiguration<AnimedDbContext>),
                typeof(DbContextOptions<AnimedDbContext>),
                typeof(DbContextOptions)
            };
            foreach (ServiceDescriptor descriptor in services.Where(d => tiposParaRemover.Contains(d.ServiceType)).ToList())
                services.Remove(descriptor);

            services.AddDbContext<AnimedDbContext>(options => options.UseSqlite(_connection));

            // Cria o schema a partir do modelo do EF (sem migrations).
#pragma warning disable ASP0000 // provider temporario apenas para criar o schema de teste
            using ServiceProvider sp = services.BuildServiceProvider();
#pragma warning restore ASP0000
            using IServiceScope scope = sp.CreateScope();
            scope.ServiceProvider.GetRequiredService<AnimedDbContext>().Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}
