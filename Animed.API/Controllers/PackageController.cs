using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Animed.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Animed.API.Controllers
{
    /// <summary>
    /// Endpoints que executam os packages PL/SQL do Animed no Oracle
    /// (entrega integrada com Mastering Relational and Non-Relational Database).
    /// </summary>
    [Authorize]
    [Route("api/package")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageAnimedUseCase _packageUseCase;

        public PackageController(IPackageAnimedUseCase packageUseCase)
        {
            _packageUseCase = packageUseCase;
        }

        [HttpPost("vacina")]
        [Authorize(Roles = "admin,veterinario")]
        [SwaggerOperation(
            Summary = "Registrar vacina pela procedure",
            Description = "Executa **PKG_ANIMED_NEGOCIO.PRC_REGISTRAR_VACINA** (tabelas TB_*): grava a vacina, credita 30 pontos, registra o histórico e o gatilho recalcula o nível do tutor."
        )]
        [SwaggerResponse(statusCode: 201, description: "Procedure executada", type: typeof(VacinaPackageModel))]
        [SwaggerResponse(statusCode: 400, description: "ORA-20011: datas inválidas")]
        [SwaggerResponse(statusCode: 404, description: "ORA-20010: pet inexistente")]
        public IActionResult PostVacina(PostVacinaPackageDto model)
        {
            var resultado = _packageUseCase.RegistrarVacina(model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("desconto")]
        [SwaggerOperation(Summary = "Calcular desconto pela função", Description = "Executa **PKG_ANIMED_NEGOCIO.FN_CALCULA_DESCONTO**.")]
        [SwaggerResponse(statusCode: 200, description: "Desconto calculado", type: typeof(GetDescontoDto))]
        public IActionResult GetDesconto(string nivel, decimal valorBruto, decimal moedas = 0)
        {
            var resultado = _packageUseCase.CalcularDesconto(nivel, valorBruto, moedas);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }

        [HttpGet("nivel/{pontos}")]
        [SwaggerOperation(Summary = "Nível do tutor pela função", Description = "Executa **PKG_ANIMED_NEGOCIO.FN_CALCULA_NIVEL**.")]
        public IActionResult GetNivel(int pontos)
        {
            var resultado = _packageUseCase.NivelPorPontos(pontos);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, new { pontos, nivel = resultado.Value });
        }

        [HttpGet("dataset")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Exportar dataset JSON", Description = "Executa **PKG_ANIMED_JSON.FN_DATASET_JSON**: o JSON é montado pelo package, sem funções nativas de JSON do Oracle.")]
        public IActionResult GetDataset()
        {
            var resultado = _packageUseCase.ExportarDatasetJson();

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return Content(resultado.Value!, "application/json");
        }
    }
}
