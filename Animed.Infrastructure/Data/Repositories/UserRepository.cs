using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;

namespace Animed.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> AdicionarAsync(UserEntity entity)
        {
            try
            {
                if (_context.Usuario.Any(x => x.Email == entity.Email))
                    throw new RegraDeNegocioException("Já existe um usuário com este e-mail");

                await _context.Usuario.AddAsync(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
            catch (RegraDeNegocioException ex)
            {
                throw new RegraDeNegocioException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<UserEntity?> AutenticarAsync(string email, string password)
        {
            try
            {
                var usuario = _context
                    .Usuario
                    .FirstOrDefault(item => item.Email == email && item.Password == password);

                if (usuario is null) throw new NoContentException("Não foi encontrado o usuario para estas credenciais");

                return await Task.FromResult(usuario);
            }
            catch (NoContentException ex)
            {
                throw new NoContentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
