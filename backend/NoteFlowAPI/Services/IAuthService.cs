namespace NoteFlowAPI.Services;

using NoteFlowAPI.DTOs;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(LoginRequestDto loginDto);
    Task RegisterAsync(RegisterRequestDto registerDto);
}

