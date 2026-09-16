namespace Sumati.Application.DTOs.Leave;

public class ApplyLeaveRequest
{
    public int LeaveTypeId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;
}