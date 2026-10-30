using Animed.Domain.Entities;

namespace Animed.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity?> AdicionarAsync(UserEntity entity);
        Task<UserEntity?> AutenticarAsync(string email, string password);
    }
}
