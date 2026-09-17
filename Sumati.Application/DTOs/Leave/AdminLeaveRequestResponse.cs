namespace Sumati.Application.DTOs.Leave;

public class AdminLeaveRequestResponse
{
    public int Id { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public string LeaveTypeName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int TotalDays { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? AdminComment { get; set; }

    public DateTime CreatedAt { get; set; }
}