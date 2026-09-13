using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;
using AnimedApi.Services;
using NSubstitute;

namespace AnimedApi.UnitTests.Services;

/// <summary>
/// Testes de unidade do PetService. Alem do CRUD, cobre a regra de negocio da camada de
/// Aplicacao: nao criar/atualizar pet apontando para um tutor inexistente.
/// </summary>
public class PetServiceTests
{
    private readonly IPetRepository _petRepository = Substitute.For<IPetRepository>();
    private readonly ITutorRepository _tutorRepository = Substitute.For<ITutorRepository>();
    private readonly PetService _service;

    public PetServiceTests() => _service = new PetService(_petRepository, _tutorRepository);

    private static PetRequest PetValido(long tutorId = 1) =>
        new() { Nome = "Rex", Especie = "CACHORRO", Raca = "SRD", Peso = 12.5m, TutorId = tutorId };

    [Fact]
    public async Task CriarAsync_QuandoTutorNaoExiste_LancaArgumentException()
    {
        // Arrange: tutor 1 nao existe no repositorio.
        _tutorRepository.BuscarPorIdAsync(1).Returns((Tutor?)null);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(PetValido()));
        await _petRepository.DidNotReceive().AdicionarAsync(Arg.Any<Pet>());
    }

    [Fact]
    public async Task CriarAsync_QuandoTutorExiste_PersisteERetornaResponse()
    {
        // Arrange
        _tutorRepository.BuscarPorIdAsync(1).Returns(new Tutor { Id = 1, Nome = "Ana", Email = "a@x.com", Cpf = "1" });
        _petRepository.When(r => r.AdicionarAsync(Arg.Any<Pet>()))
                      .Do(call => call.Arg<Pet>().Id = 5);

        // Act
        PetResponse resultado = await _service.CriarAsync(PetValido());

        // Assert
        Assert.Equal(5, resultado.Id);
        Assert.Equal("CACHORRO", resultado.Especie);
        await _petRepository.Received(1).AdicionarAsync(Arg.Is<Pet>(p => p.TutorId == 1));
    }

    [Fact]
    public async Task BuscarPorIdAsync_QuandoNaoExiste_RetornaNull()
    {
        // Arrange
        _petRepository.BuscarPorIdAsync(99).Returns((Pet?)null);

        // Act
        PetResponse? resultado = await _service.BuscarPorIdAsync(99);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoPetNaoExiste_RetornaNull()
    {
        // Arrange
        _petRepository.BuscarPorIdAsync(99).Returns((Pet?)null);

        // Act
        PetResponse? resultado = await _service.AtualizarAsync(99, PetValido());

        // Assert
        Assert.Null(resultado);
        await _petRepository.DidNotReceive().AtualizarAsync(Arg.Any<Pet>());
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTutorNaoExiste_LancaArgumentException()
    {
        // Arrange: o pet existe, mas o novo tutor informado nao.
        _petRepository.BuscarPorIdAsync(1).Returns(new Pet { Id = 1, Nome = "Rex", Especie = "CACHORRO", TutorId = 1 });
        _tutorRepository.BuscarPorIdAsync(7).Returns((Tutor?)null);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(1, PetValido(tutorId: 7)));
    }

    [Fact]
    public async Task RemoverAsync_QuandoNaoExiste_RetornaFalse()
    {
        // Arrange
        _petRepository.BuscarPorIdAsync(99).Returns((Pet?)null);

        // Act
        bool resultado = await _service.RemoverAsync(99);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ListarAsync_QuandoRecebeTutorId_RepassaFiltroAoRepositorio()
    {
        // Arrange
        _petRepository.ListarAsync(3).Returns(new List<Pet>
        {
            new() { Id = 1, Nome = "Rex", Especie = "CACHORRO", TutorId = 3 }
        });

        // Act
        List<PetResponse> resultado = await _service.ListarAsync(3);

        // Assert
        Assert.Single(resultado);
        await _petRepository.Received(1).ListarAsync(3);
    }
}
