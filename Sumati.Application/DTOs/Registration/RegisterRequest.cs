using System.ComponentModel.DataAnnotations;

namespace Sumati.Application.DTOs.Registration;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public long Phone { get; set; }

    public string Designation { get; set; } = string.Empty;

    public string EmploymentType { get; set; } = string.Empty;

    public DateTime DateOfJoining { get; set; }
}