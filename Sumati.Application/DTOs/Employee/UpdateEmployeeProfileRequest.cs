namespace Sumati.Application.DTOs.Employee;

public class UpdateEmployeeProfileRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public long Phone { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }
}