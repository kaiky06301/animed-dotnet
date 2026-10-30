using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Interfaces
{
    public interface IPetUseCase
    {
        OperationResult<PageResponseDto<GetPetDto>> ObterTodosPets(FiltroDto filtro, int? TutorId, string? Especie);
        OperationResult<GetPetDto?> ObterUmPet(int Id);
        OperationResult<GetPetDto?> AdicionarPet(PostPetDto entity);
        OperationResult<GetPetDto?> EditarPet(int Id, PostPetDto entity);
        OperationResult<GetPetDto?> DeletarPet(int Id);

        OperationResult<GetVacinaDto?> RegistrarVacina(int petId, PostVacinaDto entity);
        OperationResult<GetVacinaDto?> DeletarVacina(int petId, int vacinaId);

        OperationResult<GetConsultaDto?> RegistrarConsulta(int petId, PostConsultaDto entity);
        OperationResult<GetConsultaDto?> DeletarConsulta(int petId, int consultaId);
    }
}
