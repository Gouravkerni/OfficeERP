using Microsoft.AspNetCore.Identity;
using Sumati.Application.Interfaces;
using Sumati.Infrastructure.Identity;

namespace Sumati.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<bool> HasSuperAdminAsync()
    {
        var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");

        return superAdmins.Count > 0;
    }

    public async Task CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Unable to create role '{roleName}': {errors}");
        }
    }
}