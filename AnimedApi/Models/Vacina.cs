namespace AnimedApi.Models;

/// <summary>Vacina aplicada em um pet. Mapeada para TB_VACINA.</summary>
public class Vacina
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataAplicacao { get; set; }
    public string? Observacoes { get; set; }
    public long PetId { get; set; }
    public Pet? Pet { get; set; }

    public Vacina() { }

    /// <summary>Cria uma vacina valida; lanca DomainException se um invariante for violado.</summary>
    public static Vacina Criar(string nome, DateTime dataAplicacao, string? observacoes, long petId)
    {
        Validar(nome, dataAplicacao, petId);
        return new Vacina { Nome = nome, DataAplicacao = dataAplicacao, Observacoes = observacoes, PetId = petId };
    }

    /// <summary>Aplica novos dados a vacina, revalidando os invariantes.</summary>
    public void Atualizar(string nome, DateTime dataAplicacao, string? observacoes, long petId)
    {
        Validar(nome, dataAplicacao, petId);
        Nome = nome; DataAplicacao = dataAplicacao; Observacoes = observacoes; PetId = petId;
    }

    private static void Validar(string nome, DateTime dataAplicacao, long petId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da vacina e obrigatorio.");
        if (dataAplicacao.Date > DateTime.UtcNow.Date)
            throw new DomainException("A data de aplicacao da vacina nao pode ser futura.");
        if (petId <= 0)
            throw new DomainException("A vacina deve estar vinculada a um pet valido.");
    }
}
