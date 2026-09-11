using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Sumati.Application.Constants;
using Sumati.Infrastructure.Identity;

namespace Sumati.Infrastructure.Services;

public class SuperAdminProvisioner
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public SuperAdminProvisioner(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task ProvisionAsync()
    {
        var email = _configuration["SuperAdmin:Email"];
        var password = _configuration["SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "SuperAdmin credentials are not configured.");
        }

        var existingSuperAdmins =
            await _userManager.GetUsersInRoleAsync(Roles.SuperAdmin);

        if (existingSuperAdmins.Count > 0)
        {
            return;
        }

        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "The configured SuperAdmin email already belongs to another user.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Unable to create SuperAdmin: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            Roles.SuperAdmin);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Unable to assign SuperAdmin role: {errors}");
        }
    }
}