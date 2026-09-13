namespace AnimedApi.Models;

/// <summary>Consulta clinica de um pet. Mapeada para TB_CONSULTA.</summary>
public class Consulta
{
    public long Id { get; set; }
    public DateTime DataHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Prescricao { get; set; }
    public long PetId { get; set; }
    public Pet? Pet { get; set; }

    public Consulta() { }

    /// <summary>Cria uma consulta valida; lanca DomainException se um invariante for violado.</summary>
    public static Consulta Criar(DateTime dataHora, string motivo, string? diagnostico, string? prescricao, long petId)
    {
        Validar(dataHora, motivo, petId);
        return new Consulta { DataHora = dataHora, Motivo = motivo, Diagnostico = diagnostico, Prescricao = prescricao, PetId = petId };
    }

    /// <summary>Aplica novos dados a consulta, revalidando os invariantes.</summary>
    public void Atualizar(DateTime dataHora, string motivo, string? diagnostico, string? prescricao, long petId)
    {
        Validar(dataHora, motivo, petId);
        DataHora = dataHora; Motivo = motivo; Diagnostico = diagnostico; Prescricao = prescricao; PetId = petId;
    }

    private static void Validar(DateTime dataHora, string motivo, long petId)
    {
        if (dataHora == default)
            throw new DomainException("A data/hora da consulta e obrigatoria.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("O motivo da consulta e obrigatorio.");
        if (petId <= 0)
            throw new DomainException("A consulta deve estar vinculada a um pet valido.");
    }
}
