using Sumati.Application.DTOs.Leave;

namespace Sumati.Application.Interfaces;

public interface ILeaveService
{
    Task<LeaveRequestResponse> ApplyLeaveAsync(
        string userId,
        ApplyLeaveRequest request);

    Task<List<LeaveRequestResponse>> GetMyLeaveRequestsAsync(
        string userId);

    Task UpdateLeaveRequestStatusAsync(
    int leaveRequestId,
    string adminUserId,
    UpdateLeaveRequestStatusRequest request);

    Task<List<AdminLeaveRequestResponse>> GetAllLeaveRequestsAsync();
}