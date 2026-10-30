using Animed.API.Config;
using Animed.Infrastructure.HealthChecks;
using Animed.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetria(builder.Configuration);

builder.Services.AddLogApi();

Bootstrap.AddIoC(builder.Services, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Animed API (.NET)",
        Version = "v1",
        Description = "Cuidado contínuo do pet: tutores, pets e vacinas no Oracle e linha do tempo de cuidados no MongoDB. Challenge FIAP 2026 · Clyvo VET."
    });
    c.EnableAnnotations();
    c.ExampleFilters();

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Informe o token JWT gerado em POST /api/auth.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

//Compressao de dados
builder.Services.AddResponseCompression(options => {
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options => {
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options => {
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// Rate Limiter
builder.Services.AddRateLimiter(options => {

    // Login: 5 tentativas a cada 20 segundos
    options.AddFixedWindowLimiter(policyName: "politica_login", opt => {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(20);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Consulta de CEP: protege o serviço externo
    options.AddFixedWindowLimiter(policyName: "politica_cep", opt => {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(30);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddHealthChecks()
    // Liveness
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("API no ar"),
        tags: ["live"])
    // Readiness
    .AddOracle(
        connectionString: builder.Configuration.GetConnectionString("Oracle") ?? "",
        name: "oracle",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["db"])
    .AddCheck<MongoHealthCheck>("mongodb", failureStatus: HealthStatus.Unhealthy, tags: ["db"])
    // Servicos externos
    .AddCheck<ViaCepHealthCheck>("viacep", failureStatus: HealthStatus.Degraded, tags: ["externo"]);

// Autenticacao / Autorizacao
var key = Encoding.ASCII.GetBytes(builder.Configuration["Secretkey"]!.ToString());

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x => {
    x.RequireHttpsMetadata = false; //Em produção é true
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Tratamento global de excecoes: qualquer erro nao tratado vira um 500 padronizado
app.UseExceptionHandler(erroApp => erroApp.Run(async context =>
{
    var erro = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    Log.Error(erro, "Erro não tratado em {Metodo} {Caminho}", context.Request.Method, context.Request.Path);

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new
    {
        status = 500,
        erro = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
        rastreio = context.TraceIdentifier
    });
}));

// Log de cada requisicao com metodo, caminho, status e tempo
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/db", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db")
});

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.UseResponseCompression();

app.MapControllers();

app.Run();

public partial class Program { }
