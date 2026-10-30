using Animed.Application.Dtos;
using Animed.Domain.Entities;
using Animed.Tests.Integration.Fixtures;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Animed.Tests.Integration.Controllers
{
    [Collection("Api")]
    public class AuthControllerTest
    {
        private readonly AnimedApiFactory _factory;

        public AuthControllerTest(AnimedApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        [Trait("Integration", "Auth")]
        public async Task Post_CredenciaisInvalidas_Retorna401()
        {
            // Arrange
            _factory.UserUseCaseMock
                .Setup(x => x.AutenticarUserAsync(It.Is<PostAuthDto>(d => d.Senha == "errada")))
                .ReturnsAsync(OperationResult<GetAuthDto?>.Failure("Não foi encontrado o usuario para estas credenciais", 401));
            using var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/auth", new PostAuthDto { Email = "tutor@animed.com.br", Senha = "errada" });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        [Trait("Integration", "Auth")]
        public async Task PostUser_SemSerAdmin_Retorna403()
        {
            // Arrange
            using var client = _factory.ClienteComRole("veterinario");

            // Act
            var response = await client.PostAsJsonAsync("/api/user", new PostUserDto { Nome = "X", Email = "x@animed.com.br", Password = "123456", Role = "admin" });

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        [Trait("Integration", "Health")]
        public async Task HealthLive_ApiNoAr_Retorna200()
        {
            // Arrange
            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
        }
    }
}
