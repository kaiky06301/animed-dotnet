using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AnimedApi.Services;

/// <summary>Gera tokens JWT assinados (HMAC-SHA256) a partir das configuracoes em Jwt:*.</summary>
public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration) => _configuration = configuration;

    /// <summary>Gera um JWT valido por 1 hora, com o nome do usuario como claim.</summary>
    public string GerarToken(string usuario)
    {
        IConfigurationSection jwt = _configuration.GetSection("Jwt");
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ObterChave(jwt)));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.Name, usuario) };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // A chave real vem de configuracao (user-secrets em producao). O fallback existe so
    // para o app subir em desenvolvimento sem configuracao previa.
    private static string ObterChave(IConfigurationSection jwt) =>
        jwt["Key"] ?? "chave-de-desenvolvimento-troque-em-producao-0123456789";
}
