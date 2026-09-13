using AnimedApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AnimedApi.Middleware;

/// <summary>
/// Tratamento global de excecoes. Converte erros conhecidos em respostas HTTP adequadas
/// (400 para regra de dominio/argumento invalido) e o restante em 500, sempre em ProblemDetails.
/// Registrado com AddExceptionHandler + UseExceptionHandler no Program.cs.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    /// <summary>Trata a excecao e escreve a resposta. Retorna true quando a excecao foi tratada aqui.</summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status = exception switch
        {
            DomainException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        // So loga como erro o que for inesperado (500); 400 e fluxo normal de validacao.
        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro nao tratado ao processar {Metodo} {Rota}",
                httpContext.Request.Method, httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = status == StatusCodes.Status400BadRequest ? "Requisicao invalida" : "Erro interno",
            Detail = status == StatusCodes.Status400BadRequest ? exception.Message : "Ocorreu um erro inesperado."
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
