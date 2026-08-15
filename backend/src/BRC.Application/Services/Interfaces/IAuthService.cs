using BRC.Application.DTOs.Auth;

namespace BRC.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
