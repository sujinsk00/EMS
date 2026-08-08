namespace EmployeeManagement.Api.Models;

public enum AttendanceStatus
{
    Present,
    Absent,
    Leave,
    HalfDay
}

public class Attendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
