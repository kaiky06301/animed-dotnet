using Xunit;

namespace AnimedApi.IntegrationTests;

/// <summary>
/// Collection Fixture: compartilha uma unica instancia da API (e do banco SQLite) entre todas
/// as classes marcadas com [Collection("Integration")], evitando subir a app varias vezes.
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<AnimedWebApplicationFactory> { }
