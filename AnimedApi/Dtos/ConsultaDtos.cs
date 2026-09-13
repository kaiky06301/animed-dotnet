using System.ComponentModel.DataAnnotations;

namespace AnimedApi.Dtos;

/// <summary>Entrada para criar/atualizar consulta.</summary>
public record ConsultaRequest
{
    // DateTime? + [Required]: se o campo faltar no JSON, vira 400 (a coluna DATA_HORA
    // e NOT NULL sem default, entao a data tem que vir).
    [Required(ErrorMessage = "A data/hora e obrigatoria.")]
    public DateTime? DataHora { get; init; }

    [Required(ErrorMessage = "O motivo e obrigatorio.")]
    [StringLength(250)]
    public string Motivo { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Diagnostico { get; init; }

    [StringLength(1000)]
    public string? Prescricao { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "PetId invalido.")]
    public long PetId { get; init; }
}

/// <summary>Saida representando uma consulta.</summary>
public record ConsultaResponse(
    long Id, DateTime DataHora, string Motivo, string? Diagnostico, string? Prescricao, long PetId);
