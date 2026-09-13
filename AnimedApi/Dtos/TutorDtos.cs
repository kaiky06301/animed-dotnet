using System.ComponentModel.DataAnnotations;

namespace AnimedApi.Dtos;

/// <summary>
/// Entrada para criar/atualizar tutor. As anotacoes ([Required], [EmailAddress]...)
/// sao validadas automaticamente pelo [ApiController]: entrada invalida vira 400.
/// (Equivale ao Bean Validation da API Java.)
/// </summary>
public record TutorRequest
{
    [Required(ErrorMessage = "O nome e obrigatorio.")]
    [StringLength(120, ErrorMessage = "O nome deve ter no maximo 120 caracteres.")]
    public string Nome { get; init; } = string.Empty;

    [Required(ErrorMessage = "O e-mail e obrigatorio.")]
    [EmailAddress(ErrorMessage = "E-mail invalido.")]
    [StringLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "O CPF e obrigatorio.")]
    [StringLength(14, ErrorMessage = "O CPF deve ter no maximo 14 caracteres.")]
    public string Cpf { get; init; } = string.Empty;

    [StringLength(20)]
    public string? Telefone { get; init; }
}

/// <summary>Saida representando um tutor (record posicional: so transporte, sem validacao).</summary>
public record TutorResponse(long Id, string Nome, string Email, string Cpf, string? Telefone);
