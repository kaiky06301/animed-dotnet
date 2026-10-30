using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Interfaces
{
    public interface IHistoricoCuidadoUseCase
    {
        OperationResult<PageResponseDto<GetHistoricoCuidadoDto>> ObterTodosHistoricos(int Deslocamento, int RegistroRetornado, int? TutorId);
        OperationResult<GetHistoricoCuidadoDto?> ObterUmHistorico(string Id);
        OperationResult<GetHistoricoCuidadoDto?> ObterHistoricoDoPet(int petId);
        OperationResult<GetHistoricoCuidadoDto?> RegistrarEvento(int petId, PostEventoCuidadoDto entity);
        OperationResult<GetHistoricoCuidadoDto?> DeletarHistorico(string Id);
    }
}
