using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Interfaces
{
    public interface ITutorUseCase
    {
        OperationResult<PageResponseDto<GetTutorDto>> ObterTodosTutores(FiltroDto filtro, string? Nivel);
        OperationResult<GetTutorDto?> ObterUmTutor(int Id);
        OperationResult<GetTutorDto?> AdicionarTutor(PostTutorDto entity);
        OperationResult<GetTutorDto?> EditarTutor(int Id, PostTutorDto entity);
        OperationResult<GetTutorDto?> DeletarTutor(int Id);
        Task<OperationResult<EnderecoModel?>> ObterEnderecoPorCepAsync(string cep);
    }
}
