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

    public string Phone { get; set; } = string.Empty;

    public int DesignationId { get; set; }

    public int EmploymentTypeId { get; set; }

    public DateTime DateOfJoining { get; set; }
}