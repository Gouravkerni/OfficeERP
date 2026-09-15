namespace Sumati.Application.DTOs.Attendance;

public class MyAttendanceResponse
{
    public int AttendanceId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public string Status { get; set; } = string.Empty;
}