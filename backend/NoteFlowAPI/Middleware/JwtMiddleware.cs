namespace NoteFlowAPI.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Middleware that validates JWT tokens in request headers.
/// </summary>

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secretKey;
    
    /// <summary>
    /// Initializes a new instance of the JwtMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="config">The configuration containing JWT settings.</param>

    public JwtMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _secretKey = config["Jwt:Secret"] ?? "your_secret_key_here";
    }

    /// <summary>
    /// Processes the request by validating the JWT token.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null) AttachUserToContext(context, token);
        await _next(context);
    }
    
    /// <summary>
    /// Validates the token and attaches user information to the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <param name="token">The JWT token to validate.</param>

    private void AttachUserToContext(HttpContext context, string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = System.TimeSpan.Zero
        }, out _);
    }
}

