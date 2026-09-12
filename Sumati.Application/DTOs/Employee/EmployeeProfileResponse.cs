namespace Sumati.Application.DTOs.Employee;

public class EmployeeProfileResponse
{
    public int Id { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public long Phone { get; set; }

    public string Designation { get; set; } = string.Empty;

    public string EmploymentType { get; set; } = string.Empty;

    public DateTime DateOfJoining { get; set; }

    public string EmploymentStatus { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }
}