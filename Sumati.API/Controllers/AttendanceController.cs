using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sumati.Application.Interfaces;
using Sumati.Application.Constants;
using Sumati.Application.DTOs.Attendance;

namespace Sumati.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var response = await _attendanceService.CheckInAsync(userId);

        return Ok(response);
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var response = await _attendanceService.CheckOutAsync(userId);

        return Ok(response);
    }

    [HttpPost("mark")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> MarkAttendance(
    MarkAttendanceRequest request)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(adminUserId))
        {
            return Unauthorized();
        }

        await _attendanceService.MarkAttendanceAsync(
            adminUserId,
            request);

        return Ok(new { message = "Attendance marked successfully." });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAttendance()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var attendance = await _attendanceService
            .GetMyAttendanceAsync(userId);

        return Ok(attendance);
    }

    [HttpGet("all")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> GetAllAttendance()
    {
        var attendance = await _attendanceService.GetAllAttendanceAsync();

        return Ok(attendance);
    }

    [HttpGet("summary")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> GetMonthlyAttendanceSummary(
    int year,
    int month)
    {
        var summary = await _attendanceService
            .GetMonthlyAttendanceSummaryAsync(year, month);

        return Ok(summary);
    }

    [HttpGet("employee-summary")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> GetEmployeeMonthlyAttendanceSummary(
    int year,
    int month)
    {
        var summary = await _attendanceService
            .GetEmployeeMonthlyAttendanceSummaryAsync(year, month);

        return Ok(summary);
    }
}