using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Interfaces
{
    public interface IUserUseCase
    {
        Task<OperationResult<GetUserDto?>> AdicionarUserAsync(PostUserDto entity);
        Task<OperationResult<GetAuthDto?>> AutenticarUserAsync(PostAuthDto model);
    }
}
