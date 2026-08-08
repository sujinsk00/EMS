using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Services;

public class JwtService(IConfiguration configuration)
{
    public (string Token, DateTime ExpiresAt) CreateToken(AppUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpiresMinutes", 120));
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
