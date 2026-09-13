using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;

namespace AnimedApi.Services;

/// <summary>Regra de aplicacao de Consulta. Garante que o pet existe antes de delegar ao dominio.</summary>
public class ConsultaService
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly IPetRepository _petRepository;

    public ConsultaService(IConsultaRepository consultaRepository, IPetRepository petRepository)
    {
        _consultaRepository = consultaRepository;
        _petRepository = petRepository;
    }

    public async Task<List<ConsultaResponse>> ListarAsync(long? tutorId, long? petId, DateTime? data)
    {
        List<Consulta> consultas = await _consultaRepository.ListarAsync(tutorId, petId, data);
        return consultas.Select(ParaResponse).ToList();
    }

    public async Task<ConsultaResponse?> BuscarPorIdAsync(long id)
    {
        Consulta? consulta = await _consultaRepository.BuscarPorIdAsync(id);
        return consulta is null ? null : ParaResponse(consulta);
    }

    /// <summary>Cria uma consulta. ArgumentException se o pet nao existir.</summary>
    public async Task<ConsultaResponse> CriarAsync(ConsultaRequest request)
    {
        if (!await _petRepository.ExisteAsync(request.PetId))
            throw new ArgumentException("Pet nao encontrado para a consulta.");

        Consulta consulta = Consulta.Criar(
            request.DataHora!.Value, request.Motivo, request.Diagnostico, request.Prescricao, request.PetId);
        await _consultaRepository.AdicionarAsync(consulta);
        return ParaResponse(consulta);
    }

    public async Task<ConsultaResponse?> AtualizarAsync(long id, ConsultaRequest request)
    {
        Consulta? consulta = await _consultaRepository.BuscarPorIdAsync(id);
        if (consulta is null) return null;

        if (!await _petRepository.ExisteAsync(request.PetId))
            throw new ArgumentException("Pet nao encontrado para a consulta.");

        consulta.Atualizar(
            request.DataHora!.Value, request.Motivo, request.Diagnostico, request.Prescricao, request.PetId);
        await _consultaRepository.AtualizarAsync(consulta);
        return ParaResponse(consulta);
    }

    public async Task<bool> RemoverAsync(long id)
    {
        Consulta? consulta = await _consultaRepository.BuscarPorIdAsync(id);
        if (consulta is null) return false;

        await _consultaRepository.RemoverAsync(consulta);
        return true;
    }

    private static ConsultaResponse ParaResponse(Consulta c) =>
        new(c.Id, c.DataHora, c.Motivo, c.Diagnostico, c.Prescricao, c.PetId);
}
