using AnimedApi.Models;

namespace AnimedApi.Repositories;

/// <summary>Contrato de acesso a dados de Pet.</summary>
public interface IPetRepository
{
    Task<List<Pet>> ListarAsync(long? tutorId);
    Task<Pet?> BuscarPorIdAsync(long id);
    Task<bool> ExisteAsync(long id);
    Task AdicionarAsync(Pet pet);
    Task AtualizarAsync(Pet pet);
    Task RemoverAsync(Pet pet);
}
