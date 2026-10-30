using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Animed.Application.Mappers;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Animed.Application.UseCases
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserUseCase> _logger;

        public UserUseCase(IUserRepository userRepository, IConfiguration configuration, ILogger<UserUseCase> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<OperationResult<GetUserDto?>> AdicionarUserAsync(PostUserDto entity)
        {
            try
            {
                var result = await _userRepository.AdicionarAsync(entity.ToUserEntity());

                _logger.LogInformation("Usuario {Email} criado com a role {Role}", entity.Email, entity.Role);

                return OperationResult<GetUserDto?>.Success(result?.ToUserDto(), (int)HttpStatusCode.Created);
            }
            catch (RegraDeNegocioException ex)
            {
                return OperationResult<GetUserDto?>.Failure(ex.Message, (int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetUserDto?>.Failure("Ocorreu um erro ao adicionar o usuario", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<GetAuthDto?>> AutenticarUserAsync(PostAuthDto model)
        {
            try
            {
                var result = await _userRepository.AutenticarAsync(model.Email, model.Senha);

                //Autenticacao
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_configuration["Secretkey"]!.ToString());

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddHours(8),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, result!.Email.ToString()),
                        new Claim(ClaimTypes.Role, result!.Role.ToString()),
                    })
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var userAutenticado = new GetAuthDto
                {
                    Email = result.Email,
                    Role = result.Role,
                    Jwt = tokenHandler.WriteToken(token),
                };

                return OperationResult<GetAuthDto?>.Success(userAutenticado, (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                _logger.LogWarning("Tentativa de login sem sucesso para {Email}", model.Email);
                return OperationResult<GetAuthDto?>.Failure(ex.Message, (int)HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetAuthDto?>.Failure("Ocorreu um erro ao autenticar o usuario", (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
