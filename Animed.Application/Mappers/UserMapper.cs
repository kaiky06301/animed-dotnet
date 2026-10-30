using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Mappers
{
    public static class UserMapper
    {
        public static UserEntity ToUserEntity(this PostUserDto obj)
        {
            return new UserEntity
            {
                Nome = obj.Nome,
                Email = obj.Email,
                Password = obj.Password,
                Role = obj.Role.ToLower()
            };
        }

        public static GetUserDto ToUserDto(this UserEntity obj)
        {
            return new GetUserDto
            {
                Id = obj.Id,
                Nome = obj.Nome,
                Email = obj.Email,
                Role = obj.Role
            };
        }
    }
}
