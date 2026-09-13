using AnimedApi.Dtos;
using AnimedApi.Models;
using AnimedApi.Repositories;
using AnimedApi.Services;
using NSubstitute;

namespace AnimedApi.UnitTests.Services;

/// <summary>
/// Testes de unidade do ConsultaService. Cobre a regra: nao criar/atualizar consulta
/// para um pet inexistente (a consulta liga a PET, nao a tutor).
/// </summary>
public class ConsultaServiceTests
{
    private readonly IConsultaRepository _consultaRepository = Substitute.For<IConsultaRepository>();
    private readonly IPetRepository _petRepository = Substitute.For<IPetRepository>();
    private readonly ConsultaService _service;

    public ConsultaServiceTests() => _service = new ConsultaService(_consultaRepository, _petRepository);

    private static ConsultaRequest ConsultaValida(long petId = 1) => new()
    {
        DataHora = new DateTime(2026, 3, 10, 14, 0, 0),
        Motivo = "Check-up",
        PetId = petId
    };

    [Fact]
    public async Task CriarAsync_QuandoPetNaoExiste_LancaArgumentException()
    {
        // Arrange
        _petRepository.ExisteAsync(1).Returns(false);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(ConsultaValida()));
        await _consultaRepository.DidNotReceive().AdicionarAsync(Arg.Any<Consulta>());
    }

    [Fact]
    public async Task CriarAsync_QuandoPetExiste_PersisteERetornaResponse()
    {
        // Arrange
        _petRepository.ExisteAsync(1).Returns(true);
        _consultaRepository.When(r => r.AdicionarAsync(Arg.Any<Consulta>()))
                           .Do(call => call.Arg<Consulta>().Id = 8);

        // Act
        ConsultaResponse resultado = await _service.CriarAsync(ConsultaValida());

        // Assert
        Assert.Equal(8, resultado.Id);
        Assert.Equal("Check-up", resultado.Motivo);
        Assert.Equal(new DateTime(2026, 3, 10, 14, 0, 0), resultado.DataHora);
    }

    [Fact]
    public async Task BuscarPorIdAsync_QuandoNaoExiste_RetornaNull()
    {
        // Arrange
        _consultaRepository.BuscarPorIdAsync(99).Returns((Consulta?)null);

        // Act
        ConsultaResponse? resultado = await _service.BuscarPorIdAsync(99);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoConsultaNaoExiste_RetornaNull()
    {
        // Arrange
        _consultaRepository.BuscarPorIdAsync(99).Returns((Consulta?)null);

        // Act
        ConsultaResponse? resultado = await _service.AtualizarAsync(99, ConsultaValida());

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoPetNaoExiste_LancaArgumentException()
    {
        // Arrange
        _consultaRepository.BuscarPorIdAsync(1).Returns(new Consulta { Id = 1, Motivo = "X", PetId = 1, DataHora = DateTime.Now });
        _petRepository.ExisteAsync(1).Returns(false);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(1, ConsultaValida()));
    }

    [Fact]
    public async Task RemoverAsync_QuandoExiste_RetornaTrue()
    {
        // Arrange
        _consultaRepository.BuscarPorIdAsync(1).Returns(new Consulta { Id = 1, Motivo = "X", PetId = 1, DataHora = DateTime.Now });

        // Act
        bool resultado = await _service.RemoverAsync(1);

        // Assert
        Assert.True(resultado);
        await _consultaRepository.Received(1).RemoverAsync(Arg.Any<Consulta>());
    }

    [Fact]
    public async Task ListarAsync_RepassaFiltrosParaORepositorio()
    {
        // Arrange
        var data = new DateTime(2026, 3, 10);
        _consultaRepository.ListarAsync(null, 2, data).Returns(new List<Consulta>());

        // Act
        await _service.ListarAsync(null, 2, data);

        // Assert
        await _consultaRepository.Received(1).ListarAsync(null, 2, data);
    }
}
