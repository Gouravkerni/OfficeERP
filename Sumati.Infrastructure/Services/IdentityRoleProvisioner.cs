using Microsoft.AspNetCore.Identity;
using Sumati.Application.Constants;

namespace Sumati.Infrastructure.Services;

public class IdentityRoleProvisioner
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityRoleProvisioner(
        RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task ProvisionRolesAsync()
    {
        var roles = new[]
        {
            Roles.SuperAdmin,
            Roles.Admin,
            Roles.Employee
        };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(role));
            }
        }
    }
}