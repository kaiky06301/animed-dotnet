using AnimedApi.Dtos;
using AnimedApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimedApi.Controllers;

/// <summary>Endpoints REST de Pet. Exige autenticacao (JWT).</summary>
[ApiController]
[Authorize]
[Route("api/pets")]
public class PetsController : ControllerBase
{
    private readonly PetService _service;

    public PetsController(PetService service) => _service = service;

    /// <summary>Lista pets; ?tutorId= filtra pelos pets de um tutor.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PetResponse>>> Listar([FromQuery] long? tutorId)
        => Ok(await _service.ListarAsync(tutorId));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PetResponse>> BuscarPorId(long id)
    {
        PetResponse? pet = await _service.BuscarPorIdAsync(id);
        return pet is null ? NotFound(new { mensagem = "Pet nao encontrado." }) : Ok(pet);
    }

    /// <summary>Cria um pet. 400 se o tutor nao existir ou um invariante falhar.</summary>
    [HttpPost]
    public async Task<ActionResult<PetResponse>> Criar([FromBody] PetRequest request)
    {
        PetResponse pet = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = pet.Id }, pet);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<PetResponse>> Atualizar(long id, [FromBody] PetRequest request)
    {
        PetResponse? pet = await _service.AtualizarAsync(id, request);
        return pet is null ? NotFound(new { mensagem = "Pet nao encontrado." }) : Ok(pet);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remover(long id)
    {
        bool removido = await _service.RemoverAsync(id);
        return removido ? NoContent() : NotFound(new { mensagem = "Pet nao encontrado." });
    }
}
