using Animed.Application.Dtos;
using Animed.Application.UseCases;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Animed.Tests.Unit.Application
{
    public class UserUseCaseTest
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<ILogger<UserUseCase>> _logger = new();
        private readonly UserUseCase _userUseCase;

        public UserUseCaseTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["Secretkey"] = "chave-de-teste-com-mais-de-32-caracteres" })
                .Build();

            _userUseCase = new UserUseCase(_userRepository.Object, configuration, _logger.Object);
        }

        [Fact]
        [Trait("UseCase", "User")]
        public async Task AutenticarUserAsync_CredenciaisValidas_RetornaJwtComRole()
        {
            // Arrange
            var usuario = new UserEntity { Id = 2, Email = "doutor@animed.com.br", Password = "animed123", Role = "veterinario" };
            _userRepository.Setup(x => x.AutenticarAsync("doutor@animed.com.br", "animed123")).ReturnsAsync(usuario);

            // Act
            var resultado = await _userUseCase.AutenticarUserAsync(new PostAuthDto { Email = "doutor@animed.com.br", Senha = "animed123" });

            // Assert
            Assert.Equal(200, resultado.StatusCode);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(resultado.Value!.Jwt);
            Assert.Contains(token.Claims, c => c.Type == "role" && c.Value == "veterinario");
        }

        [Fact]
        [Trait("UseCase", "User")]
        public async Task AutenticarUserAsync_SenhaErrada_Retorna401()
        {
            // Arrange
            _userRepository.Setup(x => x.AutenticarAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new NoContentException("Não foi encontrado o usuario"));

            // Act
            var resultado = await _userUseCase.AutenticarUserAsync(new PostAuthDto { Email = "x@x.com", Senha = "errada" });

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(401, resultado.StatusCode);
        }

        [Fact]
        [Trait("UseCase", "User")]
        public async Task AdicionarUserAsync_EmailNovo_Retorna201SemExporSenha()
        {
            // Arrange
            var dto = new PostUserDto { Nome = "Ana", Email = "ana@animed.com.br", Password = "segredo1", Role = "tutor" };
            _userRepository.Setup(x => x.AdicionarAsync(It.IsAny<UserEntity>())).ReturnsAsync((UserEntity u) => { u.Id = 10; return u; });

            // Act
            var resultado = await _userUseCase.AdicionarUserAsync(dto);

            // Assert
            Assert.Equal(201, resultado.StatusCode);
            Assert.Equal(10, resultado.Value?.Id);
            Assert.DoesNotContain("Password", typeof(GetUserDto).GetProperties().Select(p => p.Name));
        }
    }
}
