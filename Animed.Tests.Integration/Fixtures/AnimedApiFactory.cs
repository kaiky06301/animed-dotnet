using Animed.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Animed.Tests.Integration.Fixtures
{
    /// <summary>
    /// Sobe a API inteira em memória (pipeline real: JWT, roles, rate limit,
    /// controllers), trocando só os casos de uso por mocks.
    /// </summary>
    public class AnimedApiFactory : WebApplicationFactory<Program>
    {
        public const string Secretkey = "chave-de-teste-de-integracao-com-32+";

        public Mock<ITutorUseCase> TutorUseCaseMock { get; } = new();
        public Mock<IPetUseCase> PetUseCaseMock { get; } = new();
        public Mock<IUserUseCase> UserUseCaseMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("Secretkey", Secretkey);
            builder.UseSetting("ConnectionStrings:Mongodb", "mongodb://localhost:27017");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(ITutorUseCase));
                services.AddSingleton(TutorUseCaseMock.Object);

                services.RemoveAll(typeof(IPetUseCase));
                services.AddSingleton(PetUseCaseMock.Object);

                services.RemoveAll(typeof(IUserUseCase));
                services.AddSingleton(UserUseCaseMock.Object);
            });
        }

        /// <summary>Cliente HTTP já autenticado com um JWT da role pedida.</summary>
        public HttpClient ClienteComRole(string role)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secretkey)), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, $"{role}@animed.com.br"),
                    new Claim(ClaimTypes.Role, role),
                })
            });

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", handler.WriteToken(token));
            return client;
        }
    }

    [CollectionDefinition("Api")]
    public class ApiCollection : ICollectionFixture<AnimedApiFactory>
    {
    }
}
