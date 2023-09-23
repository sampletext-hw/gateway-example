using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace auth_api.Jwt;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId);
}

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtConfig _config;

    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();

    public JwtTokenGenerator(IOptions<JwtConfig> options)
    {
        _config = options.Value;
    }

    public string GenerateToken(string userId)
    {
        var issuer = _config.Issuer;
        var audience = _config.Audience;

        var key = Encoding.ASCII.GetBytes(_config.Key);

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var tokenJwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            notBefore: null,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.GivenName, userId),
                // the JTI is used for our refresh token which we will be covering in the next video
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid()
                        .ToString()
                )
            },
            expires: DateTime.UtcNow.AddSeconds(_config.LifetimeSeconds),
            signingCredentials: signingCredentials
        );

        var token = JwtSecurityTokenHandler.WriteToken(tokenJwt);

        return token;
    }
}

