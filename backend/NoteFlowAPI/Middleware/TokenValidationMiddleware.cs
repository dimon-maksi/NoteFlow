namespace NoteFlowAPI.Middleware;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NoteFlowAPI.Services;

/// <summary>
/// Middleware that validates JWT tokens against the blacklist.
/// </summary>
public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenBlacklistService _tokenBlacklistService;

    /// <summary>
    /// Initializes a new instance of the TokenValidationMiddleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="tokenBlacklistService">The service that manages blacklisted tokens.</param>
    public TokenValidationMiddleware(
        RequestDelegate next,
        TokenBlacklistService tokenBlacklistService
    )
    {
        _next = next;
        _tokenBlacklistService = tokenBlacklistService;
    }

    /// <summary>
    /// Processes an HTTP request to validate the token.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token) && _tokenBlacklistService.IsTokenBlacklisted(token))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Token has been revoked");
            return;
        }

        await _next(context);
    }
}
