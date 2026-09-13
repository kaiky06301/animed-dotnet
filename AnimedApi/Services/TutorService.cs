using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;

namespace AnimedApi.Services;

/// <summary>Regra de aplicacao de Tutor: orquestra o repositorio e traduz entidade &lt;-&gt; DTO.</summary>
public class TutorService
{
    private readonly ITutorRepository _repository;

    public TutorService(ITutorRepository repository) => _repository = repository;

    /// <summary>Lista todos os tutores como DTO de saida.</summary>
    public async Task<List<TutorResponse>> ListarAsync()
    {
        List<Tutor> tutores = await _repository.ListarAsync();
        return tutores.Select(ParaResponse).ToList();
    }

    /// <summary>Busca um tutor por id; null se nao existir.</summary>
    public async Task<TutorResponse?> BuscarPorIdAsync(long id)
    {
        Tutor? tutor = await _repository.BuscarPorIdAsync(id);
        return tutor is null ? null : ParaResponse(tutor);
    }

    /// <summary>Cria um tutor (via factory de dominio) e devolve o DTO ja com o id gerado.</summary>
    public async Task<TutorResponse> CriarAsync(TutorRequest request)
    {
        Tutor tutor = Tutor.Criar(request.Nome, request.Email, request.Cpf, request.Telefone);
        await _repository.AdicionarAsync(tutor);
        return ParaResponse(tutor);
    }

    /// <summary>Atualiza um tutor existente; null se nao existir.</summary>
    public async Task<TutorResponse?> AtualizarAsync(long id, TutorRequest request)
    {
        Tutor? tutor = await _repository.BuscarPorIdAsync(id);
        if (tutor is null) return null;

        tutor.Atualizar(request.Nome, request.Email, request.Cpf, request.Telefone);
        await _repository.AtualizarAsync(tutor);
        return ParaResponse(tutor);
    }

    /// <summary>Remove um tutor; false se nao existir.</summary>
    public async Task<bool> RemoverAsync(long id)
    {
        Tutor? tutor = await _repository.BuscarPorIdAsync(id);
        if (tutor is null) return false;

        await _repository.RemoverAsync(tutor);
        return true;
    }

    private static TutorResponse ParaResponse(Tutor t) =>
        new(t.Id, t.Nome, t.Email, t.Cpf, t.Telefone);
}
