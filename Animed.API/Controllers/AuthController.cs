using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Animed.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserUseCase _userUseCase;

        public AuthController(IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        [HttpPost]
        [EnableRateLimiting("politica_login")]
        [SwaggerOperation(Summary = "Autenticar usuário", Description = "Retorna o token JWT. Use-o no botão **Authorize** do Swagger.")]
        [SwaggerResponse(statusCode: 200, description: "Token gerado", type: typeof(GetAuthDto))]
        [SwaggerResponse(statusCode: 401, description: "Credenciais inválidas")]
        public async Task<IActionResult> Post(PostAuthDto model)
        {
            var resultado = await _userUseCase.AutenticarUserAsync(model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }
    }
}
