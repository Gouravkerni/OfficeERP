using Sumati.Application.DTOs.Attendance;
using Sumati.Application.Interfaces;
using Sumati.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Sumati.Domain.Enums;
using Sumati.Domain.Entities;

namespace Sumati.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _dbContext;

    public AttendanceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckInResponse> CheckInAsync(string userId)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        var today = DateTime.UtcNow.Date;

        var attendance = await _dbContext.Attendances
            .FirstOrDefaultAsync(a =>
                a.EmployeeId == employee.Id &&
                a.AttendanceDate == today);

        if (attendance is not null)
        {
            throw new InvalidOperationException(
                "Attendance has already been marked for today.");
        }

        attendance = new Attendance
        {
            EmployeeId = employee.Id,
            AttendanceDate = today,
            CheckInTime = DateTime.UtcNow,
            Status = AttendanceStatus.Present,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Attendances.Add(attendance);

        await _dbContext.SaveChangesAsync();

        return new CheckInResponse
        {
            AttendanceId = attendance.Id,
            AttendanceDate = attendance.AttendanceDate,
            CheckInTime = attendance.CheckInTime!.Value,
            Status = attendance.Status.ToString()
        };
    }

    public async Task<CheckOutResponse> CheckOutAsync(string userId)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        var today = DateTime.UtcNow.Date;

        var attendance = await _dbContext.Attendances
            .FirstOrDefaultAsync(a =>
                a.EmployeeId == employee.Id &&
                a.AttendanceDate == today);

        if (attendance is null)
        {
            throw new InvalidOperationException(
                "You must check in before checking out.");
        }

        if (attendance.CheckOutTime is not null)
        {
            throw new InvalidOperationException(
                "Attendance has already been checked out for today.");
        }

        attendance.CheckOutTime = DateTime.UtcNow;
        attendance.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new CheckOutResponse
        {
            AttendanceId = attendance.Id,
            AttendanceDate = attendance.AttendanceDate,
            CheckInTime = attendance.CheckInTime!.Value,
            CheckOutTime = attendance.CheckOutTime.Value,
            Status = attendance.Status.ToString()
        };
    }

    public async Task MarkAttendanceAsync(
    string adminUserId,
    MarkAttendanceRequest request)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee not found.");
        }

        var attendanceDate = request.AttendanceDate.Date;

        var attendance = await _dbContext.Attendances
            .FirstOrDefaultAsync(a =>
                a.EmployeeId == employee.Id &&
                a.AttendanceDate == attendanceDate);

        if (attendance is not null)
        {
            attendance.Status = Enum.Parse<AttendanceStatus>(
                request.Status, true);

            attendance.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            attendance = new Attendance
            {
                EmployeeId = employee.Id,
                AttendanceDate = attendanceDate,
                Status = Enum.Parse<AttendanceStatus>(
                    request.Status, true),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Attendances.Add(attendance);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<MyAttendanceResponse>> GetMyAttendanceAsync(string userId)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee is null)
        {
            throw new KeyNotFoundException("Employee profile not found.");
        }

        var attendance = await _dbContext.Attendances
            .Where(a => a.EmployeeId == employee.Id)
            .OrderByDescending(a => a.AttendanceDate)
            .Select(a => new MyAttendanceResponse
            {
                AttendanceId = a.Id,
                AttendanceDate = a.AttendanceDate,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status.ToString()
            })
            .ToListAsync();

        return attendance;
    }

    public async Task<List<EmployeeAttendanceResponse>> GetAllAttendanceAsync()
    {
        var attendance = await _dbContext.Attendances
            .Join(
                _dbContext.Employees,
                attendance => attendance.EmployeeId,
                employee => employee.Id,
                (attendance, employee) => new EmployeeAttendanceResponse
                {
                    EmployeeId = employee.Id,
                    EmployeeCode = employee.EmployeeCode,
                    EmployeeName = employee.FirstName + " " + employee.LastName,
                    AttendanceDate = attendance.AttendanceDate,
                    CheckInTime = attendance.CheckInTime,
                    CheckOutTime = attendance.CheckOutTime,
                    Status = attendance.Status.ToString()
                })
            .OrderByDescending(a => a.AttendanceDate)
            .ThenBy(a => a.EmployeeCode)
            .ToListAsync();

        return attendance;
    }

    public async Task<MonthlyAttendanceSummaryResponse> GetMonthlyAttendanceSummaryAsync(
    int year,
    int month)
    {
        var attendance = await _dbContext.Attendances
            .Where(a =>
                a.AttendanceDate.Year == year &&
                a.AttendanceDate.Month == month)
            .ToListAsync();

        return new MonthlyAttendanceSummaryResponse
        {
            Year = year,
            Month = month,
            Present = attendance.Count(a => a.Status == AttendanceStatus.Present),
            Absent = attendance.Count(a => a.Status == AttendanceStatus.Absent),
            Leave = attendance.Count(a => a.Status == AttendanceStatus.Leave),
            Holiday = attendance.Count(a => a.Status == AttendanceStatus.Holiday),
            Incomplete = attendance.Count(a => a.Status == AttendanceStatus.Incomplete)
        };
    }

    public async Task<List<EmployeeMonthlyAttendanceSummaryResponse>>
    GetEmployeeMonthlyAttendanceSummaryAsync(int year, int month)
    {
        var attendance = await _dbContext.Attendances
            .Where(a =>
                a.AttendanceDate.Year == year &&
                a.AttendanceDate.Month == month)
            .ToListAsync();

        var employees = await _dbContext.Employees
            .ToListAsync();

        var result = employees.Select(employee =>
        {
            var employeeAttendance = attendance
                .Where(a => a.EmployeeId == employee.Id)
                .ToList();

            return new EmployeeMonthlyAttendanceSummaryResponse
            {
                EmployeeId = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                EmployeeName = employee.FirstName + " " + employee.LastName,

                Present = employeeAttendance.Count(
                    a => a.Status == AttendanceStatus.Present),

                Absent = employeeAttendance.Count(
                    a => a.Status == AttendanceStatus.Absent),

                Leave = employeeAttendance.Count(
                    a => a.Status == AttendanceStatus.Leave),

                Holiday = employeeAttendance.Count(
                    a => a.Status == AttendanceStatus.Holiday),

                Incomplete = employeeAttendance.Count(
                    a => a.Status == AttendanceStatus.Incomplete)
            };
        }).ToList();

        return result;
    }
}