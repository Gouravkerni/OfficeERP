namespace Sumati.Application.Interfaces;

public interface IRoleService
{
    Task<bool> RoleExistsAsync(string roleName);

    Task<bool> HasSuperAdminAsync();

    Task CreateRoleAsync(string roleName);
}