using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sumati.Application.DTOs.Leave;
using Sumati.Application.Interfaces;
using Sumati.Application.Constants;
namespace Sumati.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost]
    public async Task<IActionResult> ApplyLeave(
        ApplyLeaveRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var response = await _leaveService.ApplyLeaveAsync(
            userId,
            request);

        return Ok(response);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyLeaveRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var leaveRequests = await _leaveService
            .GetMyLeaveRequestsAsync(userId);

        return Ok(leaveRequests);
    }

    [HttpPut("{leaveRequestId}/status")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> UpdateLeaveRequestStatus(
    int leaveRequestId,
    UpdateLeaveRequestStatusRequest request)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(adminUserId))
        {
            return Unauthorized();
        }

        await _leaveService.UpdateLeaveRequestStatusAsync(
            leaveRequestId,
            adminUserId,
            request);

        return Ok(new
        {
            message = "Leave request status updated successfully."
        });
    }

    [HttpGet("all")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    public async Task<IActionResult> GetAllLeaveRequests()
    {
        var leaveRequests = await _leaveService
            .GetAllLeaveRequestsAsync();

        return Ok(leaveRequests);
    }
}