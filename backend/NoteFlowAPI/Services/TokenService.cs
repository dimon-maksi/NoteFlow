namespace NoteFlowAPI.Services;

using NoteFlowAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Service responsible for generating JWT tokens.
/// </summary>

public class TokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    

    public TokenService(IConfiguration config)
    {
        _secretKey = config["JWT_SECRET"] ?? throw new ArgumentNullException(nameof(config), "JWT_SECRET is missing from configuration");
        _issuer = config["JWT_ISSUER"] ?? throw new ArgumentNullException(nameof(config), "JWT_ISSUER is missing from configuration");
        _audience = config["JWT_AUDIENCE"] ?? throw new ArgumentNullException(nameof(config), "JWT_AUDIENCE is missing from configuration");
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A JWT token string.</returns>

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        if (key.Length < 32){
          throw new InvalidOperationException("JWT secret should more then 256 bits long");
        }


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

