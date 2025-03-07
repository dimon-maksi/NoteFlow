namespace NoteFlowAPI.Services;

using NoteFlowAPI.Data;
using NoteFlowAPI.DTOs;
using NoteFlowAPI.Models;
using System.Threading.Tasks;
using BCrypt.Net;

/// <summary>
/// Service that implements authentication and registration logic.
/// </summary>

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the AuthService class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="tokenService">Service for JWT token generation.</param>

    public AuthService(IUserRepository userRepository, TokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A JWT token if authentication succeeds; otherwise, null.</returns>

    public async Task<string?> AuthenticateAsync(LoginRequestDto loginDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
        if (user is null || !BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        return _tokenService.GenerateToken(user);
    }

    /// <summary>
    /// Registers a new user with the provided information.
    /// </summary>
    /// <param name="registerDto">The registration information.</param>

    public async Task RegisterAsync(RegisterRequestDto registerDto)
    {
        var hashedPassword = BCrypt.HashPassword(registerDto.Password);
        var user = new User { Email = registerDto.Email, PasswordHash = hashedPassword };
        await _userRepository.CreateUserAsync(user);
    }
}

