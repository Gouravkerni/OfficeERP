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

    public async Task UpdateLeaveRequestStatusAsync(
    int leaveRequestId,
    string adminUserId,
    UpdateLeaveRequestStatusRequest request)
    {
        var leaveRequest = await _dbContext.LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == leaveRequestId);

        if (leaveRequest is null)
        {
            throw new KeyNotFoundException("Leave request not found.");
        }

        if (!Enum.TryParse<LeaveRequestStatus>(
            request.Status,
            true,
            out var status))
        {
            throw new InvalidOperationException(
                "Invalid leave request status.");
        }

        if (status != LeaveRequestStatus.Approved &&
            status != LeaveRequestStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Only Approved or Rejected status is allowed.");
        }

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending leave requests can be approved or rejected.");
        }

        leaveRequest.Status = status;
        leaveRequest.AdminComment = request.AdminComment;
        leaveRequest.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<AdminLeaveRequestResponse>> GetAllLeaveRequestsAsync()
    {
        var leaveRequests = await _dbContext.LeaveRequests
            .Join(
                _dbContext.Employees,
                leaveRequest => leaveRequest.EmployeeId,
                employee => employee.Id,
                (leaveRequest, employee) => new
                {
                    LeaveRequest = leaveRequest,
                    Employee = employee
                })
            .Join(
                _dbContext.LeaveTypes,
                x => x.LeaveRequest.LeaveTypeId,
                leaveType => leaveType.Id,
                (x, leaveType) => new AdminLeaveRequestResponse
                {
                    Id = x.LeaveRequest.Id,
                    EmployeeCode = x.Employee.EmployeeCode,
                    EmployeeName = x.Employee.FirstName + " " + x.Employee.LastName,
                    LeaveTypeName = leaveType.Name,
                    StartDate = x.LeaveRequest.StartDate,
                    EndDate = x.LeaveRequest.EndDate,
                    TotalDays = (x.LeaveRequest.EndDate - x.LeaveRequest.StartDate).Days + 1,
                    Reason = x.LeaveRequest.Reason,
                    Status = x.LeaveRequest.Status.ToString(),
                    AdminComment = x.LeaveRequest.AdminComment,
                    CreatedAt = x.LeaveRequest.CreatedAt
                })
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return leaveRequests;
    }
}