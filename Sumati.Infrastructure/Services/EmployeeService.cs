using Microsoft.EntityFrameworkCore;
using Sumati.Application.DTOs.Employee;
using Sumati.Application.Interfaces;
using Sumati.Infrastructure.Persistence;

namespace Sumati.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmployeeProfileResponse> GetMyProfileAsync(string userId)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        return new EmployeeProfileResponse
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Phone = employee.Phone,
            Designation = employee.Designation,
            EmploymentType = employee.EmploymentType,
            DateOfJoining = employee.DateOfJoining,
            EmploymentStatus = employee.EmploymentStatus.ToString(),
            ProfileImageUrl = employee.ProfileImageUrl,
            Address = employee.Address,
            EmergencyContact = employee.EmergencyContact
        };
    }

    public async Task UpdateMyProfileAsync(
    string userId,
    UpdateEmployeeProfileRequest request)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Phone = request.Phone;
        employee.ProfileImageUrl = request.ProfileImageUrl;
        employee.Address = request.Address;
        employee.EmergencyContact = request.EmergencyContact;
        employee.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}