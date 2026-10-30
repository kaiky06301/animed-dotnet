using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Animed.API.Controllers
{
    [Authorize]
    [Route("api/historico")]
    [ApiController]
    public class HistoricoCuidadoController : ControllerBase
    {
        private readonly IHistoricoCuidadoUseCase _historicoUseCase;

        public HistoricoCuidadoController(IHistoricoCuidadoUseCase historicoUseCase)
        {
            _historicoUseCase = historicoUseCase;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista os históricos de cuidado (MongoDB)")]
        [SwaggerResponse(statusCode: 200, description: "Históricos listados", type: typeof(PageResponseDto<GetHistoricoCuidadoDto>))]
        public IActionResult Get(int Deslocamento, int RegistroRetornado, int? TutorId)
        {
            var resultado = _historicoUseCase.ObterTodosHistoricos(Deslocamento, RegistroRetornado, TutorId);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obter um histórico pelo ObjectId")]
        public IActionResult Get(string id)
        {
            var resultado = _historicoUseCase.ObterUmHistorico(id);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("pet/{petId}")]
        [SwaggerOperation(Summary = "Linha do tempo de cuidados de um pet")]
        public IActionResult GetPorPet(int petId)
        {
            var resultado = _historicoUseCase.ObterHistoricoDoPet(petId);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpPost("pet/{petId}/evento")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(Summary = "Registrar evento de cuidado", Description = "Adiciona um evento embutido (subdocumento) no histórico do pet.")]
        public IActionResult PostEvento(int petId, PostEventoCuidadoDto model)
        {
            var resultado = _historicoUseCase.RegistrarEvento(petId, model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Deletar histórico")]
        public IActionResult Delete(string id)
        {
            var resultado = _historicoUseCase.DeletarHistorico(id);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }
    }
}
