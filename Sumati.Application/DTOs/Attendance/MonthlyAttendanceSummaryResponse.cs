namespace Sumati.Application.DTOs.Attendance;

public class MonthlyAttendanceSummaryResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Leave { get; set; }
    public int Holiday { get; set; }
    public int Incomplete { get; set; }
}