namespace Sumati.Application.DTOs.Leave;

public class UpdateLeaveRequestStatusRequest
{
    public string Status { get; set; } = string.Empty;

    public string? AdminComment { get; set; }
}