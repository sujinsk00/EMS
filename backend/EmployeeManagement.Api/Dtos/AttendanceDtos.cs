using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Dtos;

public class AttendanceRequest
{
    public int EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public record AttendanceResponse(int Id, int EmployeeId, string EmployeeName, DateOnly Date, AttendanceStatus Status, string? Notes);
