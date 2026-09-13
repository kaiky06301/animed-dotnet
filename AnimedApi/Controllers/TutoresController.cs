using AnimedApi.Dtos;
using AnimedApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimedApi.Controllers;

/// <summary>Endpoints REST de Tutor. Exige autenticacao (JWT). Camada fina: delega ao service.</summary>
[ApiController]
[Authorize]
[Route("api/tutores")]
public class TutoresController : ControllerBase
{
    private readonly TutorService _service;

    public TutoresController(TutorService service) => _service = service;

    /// <summary>Lista todos os tutores.</summary>
    [HttpGet]
    public async Task<ActionResult<List<TutorResponse>>> Listar() => Ok(await _service.ListarAsync());

    /// <summary>Busca um tutor por id. 404 se nao existir.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<TutorResponse>> BuscarPorId(long id)
    {
        TutorResponse? tutor = await _service.BuscarPorIdAsync(id);
        return tutor is null ? NotFound(new { mensagem = "Tutor nao encontrado." }) : Ok(tutor);
    }

    /// <summary>Cria um tutor. 201 com Location; 400 se a entrada/dominio for invalido.</summary>
    [HttpPost]
    public async Task<ActionResult<TutorResponse>> Criar([FromBody] TutorRequest request)
    {
        TutorResponse tutor = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = tutor.Id }, tutor);
    }

    /// <summary>Atualiza um tutor. 404 se nao existir.</summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<TutorResponse>> Atualizar(long id, [FromBody] TutorRequest request)
    {
        TutorResponse? tutor = await _service.AtualizarAsync(id, request);
        return tutor is null ? NotFound(new { mensagem = "Tutor nao encontrado." }) : Ok(tutor);
    }

    /// <summary>Remove um tutor. 204 se removeu, 404 se nao existia.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remover(long id)
    {
        bool removido = await _service.RemoverAsync(id);
        return removido ? NoContent() : NotFound(new { mensagem = "Tutor nao encontrado." });
    }
}
