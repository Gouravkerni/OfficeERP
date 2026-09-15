using Sumati.Application.DTOs.Attendance;

namespace Sumati.Application.Interfaces;

public interface IAttendanceService
{
    Task<CheckInResponse> CheckInAsync(string userId);
    Task<CheckOutResponse> CheckOutAsync(string userId);

    Task MarkAttendanceAsync(
    string adminUserId,
    MarkAttendanceRequest request);

    Task<List<MyAttendanceResponse>> GetMyAttendanceAsync(string userId);

    Task<List<EmployeeAttendanceResponse>> GetAllAttendanceAsync();

    Task<MonthlyAttendanceSummaryResponse> GetMonthlyAttendanceSummaryAsync(int year,int month);

    Task<List<EmployeeMonthlyAttendanceSummaryResponse>> GetEmployeeMonthlyAttendanceSummaryAsync(int year,int month);
}