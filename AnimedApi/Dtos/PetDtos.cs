using System.ComponentModel.DataAnnotations;

namespace AnimedApi.Dtos;

/// <summary>Entrada para criar/atualizar pet.</summary>
public record PetRequest
{
    [Required(ErrorMessage = "O nome e obrigatorio.")]
    [StringLength(80)]
    public string Nome { get; init; } = string.Empty;

    // O banco tem CHECK nesses valores; validar aqui devolve 400 claro em vez de erro do Oracle.
    [Required(ErrorMessage = "A especie e obrigatoria.")]
    [RegularExpression("^(CACHORRO|GATO|AVE|ROEDOR|REPTIL|OUTRO)$",
        ErrorMessage = "Especie invalida. Use CACHORRO, GATO, AVE, ROEDOR, REPTIL ou OUTRO.")]
    public string Especie { get; init; } = string.Empty;

    [StringLength(80)]
    public string? Raca { get; init; }

    [Range(0, 999.99, ErrorMessage = "Peso fora do intervalo permitido.")]
    public decimal? Peso { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "TutorId invalido.")]
    public long TutorId { get; init; }
}

/// <summary>Saida representando um pet.</summary>
public record PetResponse(long Id, string Nome, string Especie, string? Raca, decimal? Peso, long TutorId);
