using Sumati.Application.DTOs.Employee;

namespace Sumati.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeProfileResponse> GetMyProfileAsync(string userId);

    Task UpdateMyProfileAsync(
    string userId,
    UpdateEmployeeProfileRequest request);
}