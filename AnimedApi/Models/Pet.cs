namespace AnimedApi.Models;

/// <summary>Pet pertencente a um tutor. Mapeado para TB_PET.</summary>
public class Pet
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public decimal? Peso { get; set; }
    public long TutorId { get; set; }
    public Tutor? Tutor { get; set; }
    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
    public ICollection<Vacina> Vacinas { get; set; } = new List<Vacina>();

    public Pet() { }

    /// <summary>Cria um pet valido; lanca DomainException se um invariante for violado.</summary>
    public static Pet Criar(string nome, string especie, string? raca, decimal? peso, long tutorId)
    {
        Validar(nome, peso, tutorId);
        return new Pet { Nome = nome, Especie = especie, Raca = raca, Peso = peso, TutorId = tutorId };
    }

    /// <summary>Aplica novos dados ao pet, revalidando os invariantes.</summary>
    public void Atualizar(string nome, string especie, string? raca, decimal? peso, long tutorId)
    {
        Validar(nome, peso, tutorId);
        Nome = nome; Especie = especie; Raca = raca; Peso = peso; TutorId = tutorId;
    }

    private static void Validar(string nome, decimal? peso, long tutorId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do pet e obrigatorio.");
        if (peso is <= 0)
            throw new DomainException("O peso do pet, quando informado, deve ser maior que zero.");
        if (tutorId <= 0)
            throw new DomainException("O pet deve estar vinculado a um tutor valido.");
    }
}
