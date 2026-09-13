using AnimedApi.Models;

namespace AnimedApi.Repositories;

/// <summary>Contrato de acesso a dados de Tutor. Permite mockar o repositorio nos testes de Aplicacao.</summary>
public interface ITutorRepository
{
    Task<List<Tutor>> ListarAsync();
    Task<Tutor?> BuscarPorIdAsync(long id);
    Task AdicionarAsync(Tutor tutor);
    Task AtualizarAsync(Tutor tutor);
    Task RemoverAsync(Tutor tutor);
}
