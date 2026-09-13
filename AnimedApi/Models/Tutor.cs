namespace AnimedApi.Models;

/// <summary>Tutor responsavel por um ou mais pets. Mapeado para TB_TUTOR.</summary>
public class Tutor
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    /// <summary>Construtor sem parametro exigido pelo EF. No codigo, prefira o factory Criar.</summary>
    public Tutor() { }

    /// <summary>Cria um tutor valido; lanca DomainException se um invariante for violado.</summary>
    public static Tutor Criar(string nome, string email, string cpf, string? telefone)
    {
        Validar(nome, email);
        return new Tutor { Nome = nome, Email = email, Cpf = cpf, Telefone = telefone };
    }

    /// <summary>Aplica novos dados ao tutor, revalidando os invariantes.</summary>
    public void Atualizar(string nome, string email, string cpf, string? telefone)
    {
        Validar(nome, email);
        Nome = nome; Email = email; Cpf = cpf; Telefone = telefone;
    }

    private static void Validar(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do tutor e obrigatorio.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O e-mail do tutor e obrigatorio.");
    }
}
