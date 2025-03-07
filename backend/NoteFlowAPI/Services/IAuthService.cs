namespace NoteFlowAPI.Services;

using NoteFlowAPI.DTOs;
using System.Threading.Tasks;

/// <summary>
/// Interface defining authentication operations.
/// </summary>

public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A JWT token if authentication succeeds; otherwise, null.</returns>
    
    Task<string?> AuthenticateAsync(LoginRequestDto loginDto);
    
    /// <summary>
    /// Registers a new user with the provided information.
    /// </summary>
    /// <param name="registerDto">The registration information.</param>

    Task RegisterAsync(RegisterRequestDto registerDto);
}

