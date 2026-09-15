namespace Sumati.Application.DTOs.Attendance;

public class EmployeeMonthlyAttendanceSummaryResponse
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    public int Present { get; set; }
    public int Absent { get; set; }
    public int Leave { get; set; }
    public int Holiday { get; set; }
    public int Incomplete { get; set; }
}