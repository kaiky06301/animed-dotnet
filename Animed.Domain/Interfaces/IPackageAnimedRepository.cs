using Animed.Domain.Entities;

namespace Animed.Domain.Interfaces
{
    /// <summary>
    /// Chamadas aos packages PL/SQL do Animed no Oracle
    /// (PKG_ANIMED_NEGOCIO e PKG_ANIMED_JSON, tabelas TB_*).
    /// </summary>
    public interface IPackageAnimedRepository
    {
        VacinaPackageModel RegistrarVacina(int petId, string nomeVacina, DateTime aplicacao, DateTime? proximaDose, string? veterinario);
        decimal? CalcularDesconto(string nivel, decimal valorBruto, decimal moedas);
        string NivelPorPontos(int pontos);
        string ExportarDatasetJson();
    }
}
