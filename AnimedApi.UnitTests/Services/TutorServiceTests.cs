using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;
using AnimedApi.Services;
using NSubstitute;

namespace AnimedApi.UnitTests.Services;

/// <summary>
/// Testes de unidade do TutorService (camada de Aplicacao). O repositorio e substituido
/// por um mock (NSubstitute), entao o teste isola a logica do service, sem tocar no banco.
/// </summary>
public class TutorServiceTests
{
    private readonly ITutorRepository _repository = Substitute.For<ITutorRepository>();
    private readonly TutorService _service;

    public TutorServiceTests() => _service = new TutorService(_repository);

    [Fact]
    public async Task ListarAsync_QuandoExistemTutores_RetornaListaMapeada()
    {
        // Arrange
        _repository.ListarAsync().Returns(new List<Tutor>
        {
            new() { Id = 1, Nome = "Ana", Email = "ana@x.com", Cpf = "111" },
            new() { Id = 2, Nome = "Bruno", Email = "bruno@x.com", Cpf = "222" }
        });

        // Act
        List<TutorResponse> resultado = await _service.ListarAsync();

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Ana", resultado[0].Nome);
        Assert.Equal(2, resultado[1].Id);
    }

    [Fact]
    public async Task BuscarPorIdAsync_QuandoExiste_RetornaResponse()
    {
        // Arrange
        _repository.BuscarPorIdAsync(1).Returns(new Tutor { Id = 1, Nome = "Ana", Email = "ana@x.com", Cpf = "111" });

        // Act
        TutorResponse? resultado = await _service.BuscarPorIdAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado!.Id);
        Assert.Equal("Ana", resultado.Nome);
    }

    [Fact]
    public async Task BuscarPorIdAsync_QuandoNaoExiste_RetornaNull()
    {
        // Arrange
        _repository.BuscarPorIdAsync(99).Returns((Tutor?)null);

        // Act
        TutorResponse? resultado = await _service.BuscarPorIdAsync(99);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task CriarAsync_QuandoValido_PersisteERetornaResponseComIdGerado()
    {
        // Arrange
        var request = new TutorRequest { Nome = "Ana", Email = "ana@x.com", Cpf = "111" };
        // Simula o banco atribuindo o id gerado durante o AdicionarAsync.
        _repository.When(r => r.AdicionarAsync(Arg.Any<Tutor>()))
                   .Do(call => call.Arg<Tutor>().Id = 10);

        // Act
        TutorResponse resultado = await _service.CriarAsync(request);

        // Assert
        Assert.Equal(10, resultado.Id);
        Assert.Equal("Ana", resultado.Nome);
        await _repository.Received(1).AdicionarAsync(Arg.Is<Tutor>(t => t.Nome == "Ana"));
    }

    [Fact]
    public async Task AtualizarAsync_QuandoNaoExiste_RetornaNull()
    {
        // Arrange
        _repository.BuscarPorIdAsync(99).Returns((Tutor?)null);

        // Act
        TutorResponse? resultado = await _service.AtualizarAsync(99, new TutorRequest { Nome = "X", Email = "x@x.com", Cpf = "1" });

        // Assert
        Assert.Null(resultado);
        await _repository.DidNotReceive().AtualizarAsync(Arg.Any<Tutor>());
    }

    [Fact]
    public async Task AtualizarAsync_QuandoExiste_AplicaAlteracoesERetornaResponse()
    {
        // Arrange
        _repository.BuscarPorIdAsync(1).Returns(new Tutor { Id = 1, Nome = "Antigo", Email = "a@x.com", Cpf = "1" });

        // Act
        TutorResponse? resultado = await _service.AtualizarAsync(1, new TutorRequest { Nome = "Novo", Email = "n@x.com", Cpf = "2" });

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Novo", resultado!.Nome);
        await _repository.Received(1).AtualizarAsync(Arg.Is<Tutor>(t => t.Nome == "Novo"));
    }

    [Fact]
    public async Task RemoverAsync_QuandoExiste_RetornaTrue()
    {
        // Arrange
        _repository.BuscarPorIdAsync(1).Returns(new Tutor { Id = 1, Nome = "Ana", Email = "a@x.com", Cpf = "1" });

        // Act
        bool resultado = await _service.RemoverAsync(1);

        // Assert
        Assert.True(resultado);
        await _repository.Received(1).RemoverAsync(Arg.Any<Tutor>());
    }

    [Fact]
    public async Task RemoverAsync_QuandoNaoExiste_RetornaFalse()
    {
        // Arrange
        _repository.BuscarPorIdAsync(99).Returns((Tutor?)null);

        // Act
        bool resultado = await _service.RemoverAsync(99);

        // Assert
        Assert.False(resultado);
        await _repository.DidNotReceive().RemoverAsync(Arg.Any<Tutor>());
    }
}
