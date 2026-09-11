using Sumati.Application.DTOs.Authentication;

namespace Sumati.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}