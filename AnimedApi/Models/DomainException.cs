namespace AnimedApi.Models;

/// <summary>
/// Erro de regra de negocio (invariante de dominio violado). O tratamento global
/// converte esta excecao em HTTP 400.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem) { }
}
