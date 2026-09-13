using System.Text;
using System.Text.Json;
using AnimedApi.Data;
using AnimedApi.HealthChecks;
using AnimedApi.Middleware;
using AnimedApi.Repositories;
using AnimedApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------- Logging estruturado (Serilog): console + arquivo com rolling diario ----------
builder.Host.UseSerilog((context, config) => config
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/animed-.log", rollingInterval: RollingInterval.Day));

// ---------- Servicos base ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + Oracle. Connection string vem de user-secrets (ConnectionStrings:AnimedDatabase).
builder.Services.AddDbContext<AnimedDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("AnimedDatabase")));

// Repositorios (interface -> implementacao) e services.
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IVacinaRepository, VacinaRepository>();
builder.Services.AddScoped<TutorService>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<VacinaService>();
builder.Services.AddScoped<TokenService>();

// ---------- Tratamento global de erros (ProblemDetails) ----------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ---------- Autenticacao JWT ----------
IConfigurationSection jwt = builder.Configuration.GetSection("Jwt");
string chaveJwt = jwt["Key"] ?? "chave-de-desenvolvimento-troque-em-producao-0123456789";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt))
        };
    });
builder.Services.AddAuthorization();

// ---------- Observabilidade: tracing + metricas (OpenTelemetry -> console) ----------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("AnimedApi"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

// ---------- Health Checks ----------
builder.Services.AddHealthChecks()
    .AddCheck<OracleHealthCheck>("oracle");

// CORS liberado para o app mobile em desenvolvimento.
builder.Services.AddCors(options =>
    options.AddPolicy("PermitirTudo", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ---------- Pipeline ----------
app.UseExceptionHandler();
app.UseSerilogRequestLogging(); // loga metodo, rota, status e tempo de cada requisicao

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirTudo");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// /health aberto (sem token), para monitoramento externo.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = EscreverRespostaHealthCheck
}).AllowAnonymous();

app.Run();

// Serializa o resultado dos health checks em JSON (status geral + cada verificacao).
static Task EscreverRespostaHealthCheck(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    var payload = new
    {
        status = report.Status.ToString(),
        duracao = report.TotalDuration.ToString(),
        verificacoes = report.Entries.Select(e => new
        {
            nome = e.Key,
            status = e.Value.Status.ToString(),
            descricao = e.Value.Description
        })
    };
    return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
}

// Torna Program acessivel ao WebApplicationFactory<Program> dos testes de integracao.
public partial class Program { }
