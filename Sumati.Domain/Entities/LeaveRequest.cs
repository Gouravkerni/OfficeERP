using Sumati.Domain.Enums;

namespace Sumati.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public LeaveRequestStatus Status { get; set; }

    public string? AdminComment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}