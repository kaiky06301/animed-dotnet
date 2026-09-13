using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;
using AnimedApi.Services;
using NSubstitute;

namespace AnimedApi.UnitTests.Services;

/// <summary>
/// Testes de unidade do VacinaService. Cobre a regra: nao registrar/atualizar vacina
/// para um pet inexistente.
/// </summary>
public class VacinaServiceTests
{
    private readonly IVacinaRepository _vacinaRepository = Substitute.For<IVacinaRepository>();
    private readonly IPetRepository _petRepository = Substitute.For<IPetRepository>();
    private readonly VacinaService _service;

    public VacinaServiceTests() => _service = new VacinaService(_vacinaRepository, _petRepository);

    private static VacinaRequest VacinaValida(long petId = 1) => new()
    {
        Nome = "V10",
        DataAplicacao = new DateTime(2026, 2, 1),
        PetId = petId
    };

    [Fact]
    public async Task CriarAsync_QuandoPetNaoExiste_LancaArgumentException()
    {
        // Arrange
        _petRepository.ExisteAsync(1).Returns(false);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(VacinaValida()));
        await _vacinaRepository.DidNotReceive().AdicionarAsync(Arg.Any<Vacina>());
    }

    [Fact]
    public async Task CriarAsync_QuandoPetExiste_PersisteERetornaResponse()
    {
        // Arrange
        _petRepository.ExisteAsync(1).Returns(true);
        _vacinaRepository.When(r => r.AdicionarAsync(Arg.Any<Vacina>()))
                         .Do(call => call.Arg<Vacina>().Id = 3);

        // Act
        VacinaResponse resultado = await _service.CriarAsync(VacinaValida());

        // Assert
        Assert.Equal(3, resultado.Id);
        Assert.Equal("V10", resultado.Nome);
    }

    [Fact]
    public async Task BuscarPorIdAsync_QuandoNaoExiste_RetornaNull()
    {
        // Arrange
        _vacinaRepository.BuscarPorIdAsync(99).Returns((Vacina?)null);

        // Act
        VacinaResponse? resultado = await _service.BuscarPorIdAsync(99);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoVacinaNaoExiste_RetornaNull()
    {
        // Arrange
        _vacinaRepository.BuscarPorIdAsync(99).Returns((Vacina?)null);

        // Act
        VacinaResponse? resultado = await _service.AtualizarAsync(99, VacinaValida());

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task RemoverAsync_QuandoNaoExiste_RetornaFalse()
    {
        // Arrange
        _vacinaRepository.BuscarPorIdAsync(99).Returns((Vacina?)null);

        // Act
        bool resultado = await _service.RemoverAsync(99);

        // Assert
        Assert.False(resultado);
    }
}
