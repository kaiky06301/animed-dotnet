using Animed.API.Doc.Samples;
using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Animed.API.Controllers
{
    [Authorize]
    [Route("api/pet")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetUseCase _petUseCase;

        public PetController(IPetUseCase petUseCase)
        {
            _petUseCase = petUseCase;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista os pets",
            Description = """
            * **TutorId:** pets de um tutor. **Especie:** CACHORRO ou GATO. **Nome:** trecho do nome.
            * **OrdenarPor:** `nome`, `peso` ou `id`. **Decrescente:** inverte a ordem.
            """
        )]
        [SwaggerResponse(statusCode: 200, description: "Pets listados com sucesso", type: typeof(PageResponseDto<GetPetDto>))]
        [SwaggerResponse(statusCode: 204, description: "Nao possui pets")]
        public IActionResult Get([FromQuery] FiltroDto filtro, int? TutorId, string? Especie)
        {
            var resultado = _petUseCase.ObterTodosPets(filtro, TutorId, Especie);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obter um pet com as vacinas")]
        [SwaggerResponse(statusCode: 200, description: "Pet encontrado", type: typeof(GetPetDto))]
        [SwaggerResponse(statusCode: 404, description: "Pet não encontrado")]
        public IActionResult Get(int id)
        {
            var resultado = _petUseCase.ObterUmPet(id);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastrar pet")]
        [SwaggerRequestExample(typeof(PostPetDto), typeof(PetRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Pet cadastrado", type: typeof(GetPetDto))]
        [SwaggerResponse(statusCode: 404, description: "Tutor não encontrado")]
        public IActionResult Post(PostPetDto model)
        {
            var resultado = _petUseCase.AdicionarPet(model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Editar pet")]
        public IActionResult Put(int id, PostPetDto model)
        {
            var resultado = _petUseCase.EditarPet(id, model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(Summary = "Deletar pet")]
        public IActionResult Delete(int id)
        {
            var resultado = _petUseCase.DeletarPet(id);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpPost("{petId}/vacina")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(
            Summary = "Registrar vacina",
            Description = "Somente **veterinário/admin**. Salva no Oracle, credita **30 pontos** ao tutor e grava o evento no histórico do MongoDB."
        )]
        [SwaggerResponse(statusCode: 201, description: "Vacina registrada", type: typeof(GetVacinaDto))]
        [SwaggerResponse(statusCode: 400, description: "Data inválida")]
        [SwaggerResponse(statusCode: 404, description: "Pet não encontrado")]
        public IActionResult PostVacina(int petId, PostVacinaDto model)
        {
            var resultado = _petUseCase.RegistrarVacina(petId, model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpDelete("{petId}/vacina/{vacinaId}")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(Summary = "Deletar vacina")]
        public IActionResult DeleteVacina(int petId, int vacinaId)
        {
            var resultado = _petUseCase.DeletarVacina(petId, vacinaId);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }
    
        [HttpPost("{petId}/consulta")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(
            Summary = "Registrar consulta",
            Description = "Somente **veterinário/admin**. Salva no Oracle, credita **20 pontos** ao tutor e grava o evento no histórico do MongoDB."
        )]
        [SwaggerResponse(statusCode: 201, description: "Consulta registrada", type: typeof(GetConsultaDto))]
        [SwaggerResponse(statusCode: 404, description: "Pet não encontrado")]
        public IActionResult PostConsulta(int petId, PostConsultaDto model)
        {
            var resultado = _petUseCase.RegistrarConsulta(petId, model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpDelete("{petId}/consulta/{consultaId}")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(Summary = "Deletar consulta")]
        public IActionResult DeleteConsulta(int petId, int consultaId)
        {
            var resultado = _petUseCase.DeletarConsulta(petId, consultaId);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }
    }
}
