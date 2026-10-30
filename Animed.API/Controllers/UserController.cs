using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Animed.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserUseCase _userUseCase;

        public UserController(IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Cadastrar usuário", Description = "Somente **admin** cria usuários (roles: tutor, veterinario, admin).")]
        [SwaggerResponse(statusCode: 201, description: "Usuário criado", type: typeof(GetUserDto))]
        [SwaggerResponse(statusCode: 409, description: "E-mail já cadastrado")]
        public async Task<IActionResult> Post(PostUserDto model)
        {
            var resultado = await _userUseCase.AdicionarUserAsync(model);

            if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);

            return StatusCode(resultado.StatusCode, resultado.Value);
        }
    }
}
