using Animed.API.Doc.Samples;
using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Animed.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Animed.API.Controllers
{
    [Authorize]
    [Route("api/tutor")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutorUseCase _tutorUseCase;

        public TutorController(ITutorUseCase tutorUseCase)
        {
            _tutorUseCase = tutorUseCase;
        }

        [HttpGet]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(
            Summary = "Lista os tutores",
            Description = """
            ## 📋 Paginação, filtros e ordenação
            * **Deslocamento / RegistroRetornado:** paginação (offset).
            * **Nome:** filtra pelo trecho do nome. **Nivel:** BASICO, CUIDADOR ou TUTOR_PREMIUM.
            * **OrdenarPor:** `nome`, `pontos` ou `id` (padrão). **Decrescente:** inverte a ordem.

            ## 📋 Retorno
            * **200:** página com links HATEOAS (self, next_page, previous_page).
            * **204:** nenhum tutor para o filtro.
            """
        )]
        [SwaggerResponse(statusCode: 200, description: "Tutores listados com sucesso", type: typeof(PageResponseDto<GetTutorDto>))]
        [SwaggerResponse(statusCode: 204, description: "Nao possui tutores")]
        [SwaggerResponseExample(statusCode: 200, typeof(TutorResponseListSample))]
        public IActionResult Get([FromQuery] FiltroDto filtro, string? Nivel)
        {
            var resultado = _tutorUseCase.ObterTodosTutores(filtro, Nivel);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obter um tutor")]
        [SwaggerResponse(statusCode: 200, description: "Tutor encontrado", type: typeof(GetTutorDto))]
        [SwaggerResponse(statusCode: 404, description: "Tutor não encontrado")]
        public IActionResult Get(int id)
        {
            var resultado = _tutorUseCase.ObterUmTutor(id);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpPost]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(Summary = "Cadastrar tutor", Description = "O tutor começa com 0 pontos no nível BASICO.")]
        [SwaggerRequestExample(typeof(PostTutorDto), typeof(TutorRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Tutor cadastrado", type: typeof(GetTutorDto))]
        [SwaggerResponse(statusCode: 409, description: "E-mail já cadastrado")]
        public IActionResult Post(PostTutorDto model)
        {
            var resultado = _tutorUseCase.AdicionarTutor(model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Editar tutor")]
        public IActionResult Put([FromRoute, SwaggerParameter("Id do tutor")] int id, PostTutorDto model)
        {
            var resultado = _tutorUseCase.EditarTutor(id, model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Deletar tutor", Description = "Somente **admin**. Remove também os pets e vacinas do tutor.")]
        public IActionResult Delete(int id)
        {
            var resultado = _tutorUseCase.DeletarTutor(id);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("endereco/{cep}")]
        [AllowAnonymous]
        [EnableRateLimiting("politica_cep")]
        [SwaggerOperation(Summary = "Obter endereço por CEP", Description = "Consulta o serviço externo **ViaCEP**.")]
        [SwaggerResponse(statusCode: 200, description: "Endereço encontrado", type: typeof(EnderecoModel))]
        [SwaggerResponse(statusCode: 404, description: "CEP não encontrado")]
        public async Task<IActionResult> GetEnderecoPorCep(string cep)
        {
            var resultado = await _tutorUseCase.ObterEnderecoPorCepAsync(cep);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }
    }
}
