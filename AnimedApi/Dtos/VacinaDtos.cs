using System.ComponentModel.DataAnnotations;

namespace AnimedApi.Dtos;

/// <summary>Entrada para criar/atualizar vacina.</summary>
public record VacinaRequest
{
    [Required(ErrorMessage = "O nome da vacina e obrigatorio.")]
    [StringLength(100)]
    public string Nome { get; init; } = string.Empty;

    // DATA_APLICACAO e NOT NULL sem default: obrigatoria na entrada.
    [Required(ErrorMessage = "A data de aplicacao e obrigatoria.")]
    public DateTime? DataAplicacao { get; init; }

    [StringLength(500)]
    public string? Observacoes { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "PetId invalido.")]
    public long PetId { get; init; }
}

/// <summary>Saida representando uma vacina.</summary>
public record VacinaResponse(long Id, string Nome, DateTime DataAplicacao, string? Observacoes, long PetId);
