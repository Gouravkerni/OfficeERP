using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sumati.Domain.Entities;
using Sumati.Infrastructure.Identity;

namespace Sumati.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>()
            .HasIndex(employee => employee.EmployeeCode)
            .IsUnique();

        builder.Entity<Employee>()
            .HasIndex(employee => employee.UserId)
            .IsUnique();

        builder.Entity<Employee>()
    .HasOne<ApplicationUser>()
    .WithOne()
    .HasForeignKey<Employee>(employee => employee.UserId)
    .OnDelete(DeleteBehavior.Cascade); 

        builder.Entity<Attendance>()
    .HasOne<Employee>()
    .WithMany()
    .HasForeignKey(attendance => attendance.EmployeeId)
    .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Attendance>()
    .HasIndex(attendance => new
    {
        attendance.EmployeeId,
        attendance.AttendanceDate
    })
    .IsUnique();
    }
}