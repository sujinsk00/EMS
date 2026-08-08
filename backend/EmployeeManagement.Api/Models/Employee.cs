namespace EmployeeManagement.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime JoiningDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
