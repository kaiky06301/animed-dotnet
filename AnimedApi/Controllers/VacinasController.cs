using AnimedApi.Dtos;
using AnimedApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimedApi.Controllers;

/// <summary>Endpoints REST de Vacina. Exige autenticacao (JWT).</summary>
[ApiController]
[Authorize]
[Route("api/vacinas")]
public class VacinasController : ControllerBase
{
    private readonly VacinaService _service;

    public VacinasController(VacinaService service) => _service = service;

    /// <summary>Lista vacinas; ?petId= filtra pelas vacinas de um pet.</summary>
    [HttpGet]
    public async Task<ActionResult<List<VacinaResponse>>> Listar([FromQuery] long? petId)
        => Ok(await _service.ListarAsync(petId));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<VacinaResponse>> BuscarPorId(long id)
    {
        VacinaResponse? vacina = await _service.BuscarPorIdAsync(id);
        return vacina is null ? NotFound(new { mensagem = "Vacina nao encontrada." }) : Ok(vacina);
    }

    /// <summary>Registra uma vacina. 400 se o pet nao existir ou um invariante falhar.</summary>
    [HttpPost]
    public async Task<ActionResult<VacinaResponse>> Criar([FromBody] VacinaRequest request)
    {
        VacinaResponse vacina = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = vacina.Id }, vacina);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<VacinaResponse>> Atualizar(long id, [FromBody] VacinaRequest request)
    {
        VacinaResponse? vacina = await _service.AtualizarAsync(id, request);
        return vacina is null ? NotFound(new { mensagem = "Vacina nao encontrada." }) : Ok(vacina);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remover(long id)
    {
        bool removido = await _service.RemoverAsync(id);
        return removido ? NoContent() : NotFound(new { mensagem = "Vacina nao encontrada." });
    }
}
