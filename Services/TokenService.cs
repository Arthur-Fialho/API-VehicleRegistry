using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VehicleRegistryAPI.Models;

namespace VehicleRegistryAPI.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // As 'claims' são as informações que guardaremos no token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // Cria a chave simétrica para assinar o token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("Jwt:Key").Value!));

        // Cria as credenciais de assinatura
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        // Cria o token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2), // Token expira em 2 horas
            signingCredentials: creds
        );

        // Gera o token em formato string
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}