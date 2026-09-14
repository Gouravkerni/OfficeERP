namespace Sumati.Application.DTOs.Attendance;

public class CheckInResponse
{
    public int AttendanceId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public DateTime CheckInTime { get; set; }

    public string Status { get; set; } = string.Empty;
}