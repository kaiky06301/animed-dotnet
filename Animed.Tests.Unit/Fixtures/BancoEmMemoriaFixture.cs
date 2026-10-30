using Animed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Animed.Tests.Unit.Fixtures
{
    /// <summary>
    /// Cria um ApplicationContext em memória. Cada teste pede um banco novo
    /// com nome próprio, então os dados de um teste não vazam para outro.
    /// </summary>
    public class BancoEmMemoriaFixture
    {
        public ApplicationContext NovoContexto()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: $"AnimedTeste_{Guid.NewGuid()}")
                .Options;

            var context = new ApplicationContext(options);
            context.Database.EnsureCreated();

            return context;
        }
    }

    [CollectionDefinition("BancoEmMemoria")]
    public class BancoEmMemoriaCollection : ICollectionFixture<BancoEmMemoriaFixture>
    {
    }
}
