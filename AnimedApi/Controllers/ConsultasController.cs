using AnimedApi.Dtos;
using AnimedApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimedApi.Controllers;

/// <summary>Endpoints REST de Consulta. Exige autenticacao (JWT).</summary>
[ApiController]
[Authorize]
[Route("api/consultas")]
public class ConsultasController : ControllerBase
{
    private readonly ConsultaService _service;

    public ConsultasController(ConsultaService service) => _service = service;

    /// <summary>Lista consultas com filtros opcionais por tutor, pet e dia (?data=2026-03-10).</summary>
    [HttpGet]
    public async Task<ActionResult<List<ConsultaResponse>>> Listar(
        [FromQuery] long? tutorId, [FromQuery] long? petId, [FromQuery] DateTime? data)
        => Ok(await _service.ListarAsync(tutorId, petId, data));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ConsultaResponse>> BuscarPorId(long id)
    {
        ConsultaResponse? consulta = await _service.BuscarPorIdAsync(id);
        return consulta is null ? NotFound(new { mensagem = "Consulta nao encontrada." }) : Ok(consulta);
    }

    /// <summary>Cria uma consulta. 400 se o pet nao existir ou um invariante falhar.</summary>
    [HttpPost]
    public async Task<ActionResult<ConsultaResponse>> Criar([FromBody] ConsultaRequest request)
    {
        ConsultaResponse consulta = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = consulta.Id }, consulta);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ConsultaResponse>> Atualizar(long id, [FromBody] ConsultaRequest request)
    {
        ConsultaResponse? consulta = await _service.AtualizarAsync(id, request);
        return consulta is null ? NotFound(new { mensagem = "Consulta nao encontrada." }) : Ok(consulta);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remover(long id)
    {
        bool removido = await _service.RemoverAsync(id);
        return removido ? NoContent() : NotFound(new { mensagem = "Consulta nao encontrada." });
    }
}
