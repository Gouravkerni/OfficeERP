namespace Sumati.Application.DTOs.Attendance;

public class MarkAttendanceRequest
{
    public int EmployeeId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public string Status { get; set; } = string.Empty;
}