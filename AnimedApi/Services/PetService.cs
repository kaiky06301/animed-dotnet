using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;

namespace AnimedApi.Services;

/// <summary>Regra de aplicacao de Pet. Valida a existencia do tutor (regra de aplicacao)
/// antes de delegar a criacao/atualizacao ao dominio.</summary>
public class PetService
{
    private readonly IPetRepository _petRepository;
    private readonly ITutorRepository _tutorRepository;

    public PetService(IPetRepository petRepository, ITutorRepository tutorRepository)
    {
        _petRepository = petRepository;
        _tutorRepository = tutorRepository;
    }

    public async Task<List<PetResponse>> ListarAsync(long? tutorId)
    {
        List<Pet> pets = await _petRepository.ListarAsync(tutorId);
        return pets.Select(ParaResponse).ToList();
    }

    public async Task<PetResponse?> BuscarPorIdAsync(long id)
    {
        Pet? pet = await _petRepository.BuscarPorIdAsync(id);
        return pet is null ? null : ParaResponse(pet);
    }

    /// <summary>Cria um pet. ArgumentException se o tutor nao existir; DomainException se um invariante falhar.</summary>
    public async Task<PetResponse> CriarAsync(PetRequest request)
    {
        if (await _tutorRepository.BuscarPorIdAsync(request.TutorId) is null)
            throw new ArgumentException("Tutor nao encontrado para o pet.");

        Pet pet = Pet.Criar(request.Nome, request.Especie, request.Raca, request.Peso, request.TutorId);
        await _petRepository.AdicionarAsync(pet);
        return ParaResponse(pet);
    }

    public async Task<PetResponse?> AtualizarAsync(long id, PetRequest request)
    {
        Pet? pet = await _petRepository.BuscarPorIdAsync(id);
        if (pet is null) return null;

        if (await _tutorRepository.BuscarPorIdAsync(request.TutorId) is null)
            throw new ArgumentException("Tutor nao encontrado para o pet.");

        pet.Atualizar(request.Nome, request.Especie, request.Raca, request.Peso, request.TutorId);
        await _petRepository.AtualizarAsync(pet);
        return ParaResponse(pet);
    }

    public async Task<bool> RemoverAsync(long id)
    {
        Pet? pet = await _petRepository.BuscarPorIdAsync(id);
        if (pet is null) return false;

        await _petRepository.RemoverAsync(pet);
        return true;
    }

    private static PetResponse ParaResponse(Pet p) =>
        new(p.Id, p.Nome, p.Especie, p.Raca, p.Peso, p.TutorId);
}
