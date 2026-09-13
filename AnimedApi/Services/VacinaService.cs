using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;

namespace AnimedApi.Services;

/// <summary>Regra de aplicacao de Vacina. Garante que o pet existe antes de delegar ao dominio.</summary>
public class VacinaService
{
    private readonly IVacinaRepository _vacinaRepository;
    private readonly IPetRepository _petRepository;

    public VacinaService(IVacinaRepository vacinaRepository, IPetRepository petRepository)
    {
        _vacinaRepository = vacinaRepository;
        _petRepository = petRepository;
    }

    public async Task<List<VacinaResponse>> ListarAsync(long? petId)
    {
        List<Vacina> vacinas = await _vacinaRepository.ListarAsync(petId);
        return vacinas.Select(ParaResponse).ToList();
    }

    public async Task<VacinaResponse?> BuscarPorIdAsync(long id)
    {
        Vacina? vacina = await _vacinaRepository.BuscarPorIdAsync(id);
        return vacina is null ? null : ParaResponse(vacina);
    }

    /// <summary>Registra uma vacina. ArgumentException se o pet nao existir.</summary>
    public async Task<VacinaResponse> CriarAsync(VacinaRequest request)
    {
        if (!await _petRepository.ExisteAsync(request.PetId))
            throw new ArgumentException("Pet nao encontrado para a vacina.");

        Vacina vacina = Vacina.Criar(request.Nome, request.DataAplicacao!.Value, request.Observacoes, request.PetId);
        await _vacinaRepository.AdicionarAsync(vacina);
        return ParaResponse(vacina);
    }

    public async Task<VacinaResponse?> AtualizarAsync(long id, VacinaRequest request)
    {
        Vacina? vacina = await _vacinaRepository.BuscarPorIdAsync(id);
        if (vacina is null) return null;

        if (!await _petRepository.ExisteAsync(request.PetId))
            throw new ArgumentException("Pet nao encontrado para a vacina.");

        vacina.Atualizar(request.Nome, request.DataAplicacao!.Value, request.Observacoes, request.PetId);
        await _vacinaRepository.AtualizarAsync(vacina);
        return ParaResponse(vacina);
    }

    public async Task<bool> RemoverAsync(long id)
    {
        Vacina? vacina = await _vacinaRepository.BuscarPorIdAsync(id);
        if (vacina is null) return false;

        await _vacinaRepository.RemoverAsync(vacina);
        return true;
    }

    private static VacinaResponse ParaResponse(Vacina v) =>
        new(v.Id, v.Nome, v.DataAplicacao, v.Observacoes, v.PetId);
}
