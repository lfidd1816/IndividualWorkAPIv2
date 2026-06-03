using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IndividualWorkAPI.UniversalMethods;

public class JWTTokenGenerator
{
    private readonly string _secretKey;

    public JWTTokenGenerator(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"] ?? throw new Exception("Jwt не найден"); 
    }

    public string GenerateToken(int userId, int roleId)
    {
        var claims = new[]
        {
            new Claim("userId",userId.ToString()),
            new Claim("roleId", roleId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer),

        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}