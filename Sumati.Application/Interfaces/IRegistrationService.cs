using Sumati.Application.DTOs.Registration;

namespace Sumati.Application.Interfaces;

public interface IRegistrationService
{
    Task RegisterAdminAsync(RegisterRequest request);

    Task RegisterEmployeeAsync(RegisterRequest request);
}