using AnimedApi.Models;

namespace AnimedApi.Repositories;

/// <summary>Contrato de acesso a dados de Vacina.</summary>
public interface IVacinaRepository
{
    Task<List<Vacina>> ListarAsync(long? petId);
    Task<Vacina?> BuscarPorIdAsync(long id);
    Task AdicionarAsync(Vacina vacina);
    Task AtualizarAsync(Vacina vacina);
    Task RemoverAsync(Vacina vacina);
}
