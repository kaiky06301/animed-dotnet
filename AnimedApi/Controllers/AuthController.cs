using AnimedApi.Dtos;
using AnimedApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimedApi.Controllers;

/// <summary>Autenticacao: troca usuario/senha por um token JWT. Aberto (sem token).</summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthController(TokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    /// <summary>Login. Credenciais validas devolvem 200 + token; invalidas devolvem 401.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        IConfigurationSection auth = _configuration.GetSection("Auth");
        string usuarioEsperado = auth["Usuario"] ?? "admin";
        string senhaEsperada = auth["Senha"] ?? "admin";

        if (request.Usuario != usuarioEsperado || request.Senha != senhaEsperada)
            return Unauthorized(new { mensagem = "Usuario ou senha invalidos." });

        string token = _tokenService.GerarToken(request.Usuario);
        return Ok(new LoginResponse(token, DateTime.UtcNow.AddHours(1)));
    }
}
