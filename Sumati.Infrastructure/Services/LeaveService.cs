using Microsoft.EntityFrameworkCore;
using Sumati.Application.DTOs.Leave;
using Sumati.Application.Interfaces;
using Sumati.Domain.Entities;
using Sumati.Domain.Enums;
using Sumati.Infrastructure.Persistence;

namespace Sumati.Infrastructure.Services;

public class LeaveService : ILeaveService
{
    private readonly ApplicationDbContext _dbContext;

    public LeaveService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LeaveRequestResponse> ApplyLeaveAsync(
        string userId,
        ApplyLeaveRequest request)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        var leaveType = await _dbContext.LeaveTypes
            .FirstOrDefaultAsync(l => l.Id == request.LeaveTypeId);

        if (leaveType is null || !leaveType.IsActive)
        {
            throw new KeyNotFoundException("Leave type not found or inactive.");
        }

        if (request.StartDate.Date > request.EndDate.Date)
        {
            throw new InvalidOperationException(
                "Start date cannot be after end date.");
        }

        var totalDays =
            (request.EndDate.Date - request.StartDate.Date).Days + 1;

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employee.Id,
            LeaveTypeId = leaveType.Id,
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,
            Reason = request.Reason,
            Status = LeaveRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.LeaveRequests.Add(leaveRequest);

        await _dbContext.SaveChangesAsync();

        return new LeaveRequestResponse
        {
            Id = leaveRequest.Id,
            LeaveTypeId = leaveType.Id,
            LeaveTypeName = leaveType.Name,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            TotalDays = totalDays,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status.ToString(),
            AdminComment = leaveRequest.AdminComment,
            CreatedAt = leaveRequest.CreatedAt
        };
    }

    public async Task<List<LeaveRequestResponse>> GetMyLeaveRequestsAsync(
     string userId)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        var leaveRequests = await _dbContext.LeaveRequests
            .Where(l => l.EmployeeId == employee.Id)
            .Join(
                _dbContext.LeaveTypes,
                leaveRequest => leaveRequest.LeaveTypeId,
                leaveType => leaveType.Id,
                (leaveRequest, leaveType) => new LeaveRequestResponse
                {
                    Id = leaveRequest.Id,
                    LeaveTypeId = leaveType.Id,
                    LeaveTypeName = leaveType.Name,
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    TotalDays = (leaveRequest.EndDate - leaveRequest.StartDate).Days + 1,
                    Reason = leaveRequest.Reason,
                    Status = leaveRequest.Status.ToString(),
                    AdminComment = leaveRequest.AdminComment,
                    CreatedAt = leaveRequest.CreatedAt
                })
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return leaveRequests;
    }
}