using Sumati.Application.DTOs.Attendance;

namespace Sumati.Application.Interfaces;

public interface IAttendanceService
{
    Task<CheckInResponse> CheckInAsync(string userId);
    Task<CheckOutResponse> CheckOutAsync(string userId);

    Task MarkAttendanceAsync(
    string adminUserId,
    MarkAttendanceRequest request);
}