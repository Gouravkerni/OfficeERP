namespace Sumati.Domain.Entities;

using Sumati.Domain.Enums;

public class Employee
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
  
    public string EmployeeCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int DesignationId { get; set; }

    public int EmploymentTypeId { get; set; }

    public DateTime DateOfJoining { get; set; }

    public EmploymentStatus EmploymentStatus { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}