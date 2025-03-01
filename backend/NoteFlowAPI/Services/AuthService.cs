namespace NoteFlowAPI.Services;

using NoteFlowAPI.Data;
using NoteFlowAPI.DTOs;
using NoteFlowAPI.Models;
using System.Threading.Tasks;
using BCrypt.Net;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;

    public AuthService(IUserRepository userRepository, TokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<string?> AuthenticateAsync(LoginRequestDto loginDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
        if (user is null || !BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        return _tokenService.GenerateToken(user);
    }

    public async Task RegisterAsync(RegisterRequestDto registerDto)
    {
        var hashedPassword = BCrypt.HashPassword(registerDto.Password);
        var user = new User { Email = registerDto.Email, PasswordHash = hashedPassword };
        await _userRepository.CreateUserAsync(user);
    }
}

