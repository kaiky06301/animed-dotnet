using AnimedApi.Models;

namespace AnimedApi.Repositories;

/// <summary>Contrato de acesso a dados de Consulta.</summary>
public interface IConsultaRepository
{
    Task<List<Consulta>> ListarAsync(long? tutorId, long? petId, DateTime? data);
    Task<Consulta?> BuscarPorIdAsync(long id);
    Task AdicionarAsync(Consulta consulta);
    Task AtualizarAsync(Consulta consulta);
    Task RemoverAsync(Consulta consulta);
}
