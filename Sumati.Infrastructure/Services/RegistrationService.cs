using Microsoft.AspNetCore.Identity;
using Sumati.Application.Constants;
using Sumati.Application.DTOs.Registration;
using Sumati.Application.Interfaces;
using Sumati.Domain.Entities;
using Sumati.Domain.Enums;
using Sumati.Infrastructure.Identity;
using Sumati.Infrastructure.Persistence;

namespace Sumati.Infrastructure.Services;

public class RegistrationService : IRegistrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public RegistrationService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task RegisterAdminAsync(RegisterRequest request)
    {
        await CreateUserAsync(request, Roles.Admin);
    }

    public async Task RegisterEmployeeAsync(RegisterRequest request)
    {
        var user = await CreateUserAsync(request, Roles.Employee);

        var employee = new Employee
        {
            UserId = user.Id,
            EmployeeCode = $"EMP-{Guid.NewGuid():N}".ToUpper(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            DesignationId = request.DesignationId,
            EmploymentTypeId = request.EmploymentTypeId,
            DateOfJoining = request.DateOfJoining,
            EmploymentStatus = EmploymentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Employees.Add(employee);

        await _dbContext.SaveChangesAsync();
    }

    private async Task<ApplicationUser> CreateUserAsync(
    RegisterRequest request,
    string role)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Unable to create user: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            role);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(error => error.Description));

            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                $"Unable to assign role: {errors}");
        }

        return user;
    }
}