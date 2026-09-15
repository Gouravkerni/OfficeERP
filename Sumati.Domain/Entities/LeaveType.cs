namespace Sumati.Domain.Entities;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxDaysPerYear { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}