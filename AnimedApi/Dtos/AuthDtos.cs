using System.ComponentModel.DataAnnotations;

namespace AnimedApi.Dtos;

/// <summary>Credenciais de login.</summary>
public record LoginRequest
{
    [Required(ErrorMessage = "O usuario e obrigatorio.")]
    public string Usuario { get; init; } = string.Empty;

    [Required(ErrorMessage = "A senha e obrigatoria.")]
    public string Senha { get; init; } = string.Empty;
}

/// <summary>Token emitido no login bem-sucedido.</summary>
public record LoginResponse(string Token, DateTime ExpiraEm);
