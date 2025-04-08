namespace NoteFlowAPI.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NoteFlowAPI.Models;

/// <summary>
/// Service responsible for generating JWT tokens.
/// </summary>

public class TokenService
{
    private readonly string _secretKey;
    private readonly string? _issuer;
    private readonly string? _audience;

    private readonly TimeSpan _default = TimeSpan.FromHours(2);
    private readonly TimeSpan _extended = TimeSpan.FromDays(30);

    /// <summary>
    /// Initializes a new instance of the TokenService class.
    /// </summary>
    /// <param name="config">The application configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when JWT_SECRET environment variable is missing.</exception>

    public TokenService(IConfiguration config)
    {
        _secretKey =
            config["JWT_SECRET"]
            ?? throw new ArgumentNullException("JWT_SECRET is missing");
        _issuer = config["JWT_ISSUER"];
        _audience = config["JWT_AUDIENCE"];
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A JWT token string.</returns>

    public string GenerateToken(User user, bool rememberMe = false)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        if (key.Length < 32)
        {
            throw new InvalidOperationException("JWT secret should more then 256 bits long");
        }

        TimeSpan expirationTime = rememberMe ? _extended : _default;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(expirationTime),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
