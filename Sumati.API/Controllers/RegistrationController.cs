using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sumati.Application.Constants;
using Sumati.Application.DTOs.Registration;
using Sumati.Application.Interfaces;

namespace Sumati.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationController(
        IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost("admin")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> RegisterAdmin(
        RegisterRequest request)
    {
        await _registrationService.RegisterAdminAsync(request);

        return Ok(new
        {
            message = "Admin registered successfully."
        });
    }

    [HttpPost("employee")]
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
    public async Task<IActionResult> RegisterEmployee(
        RegisterRequest request)
    {
        await _registrationService.RegisterEmployeeAsync(request);

        return Ok(new
        {
            message = "Employee registered successfully."
        });
    }
}