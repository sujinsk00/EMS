using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Dtos;

public class EmployeeCreateRequest
{
    [Required, MaxLength(30)] public string EmployeeCode { get; set; } = string.Empty;
    [Required, MaxLength(80)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(80)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(160)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(30)] public string Phone { get; set; } = string.Empty;
    [Required, MaxLength(120)] public string JobTitle { get; set; } = string.Empty;
    [Range(0, 100000000)] public decimal Salary { get; set; }
    public DateTime JoiningDate { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class EmployeeUpdateRequest : EmployeeCreateRequest { }

public record EmployeeResponse(
    int Id,
    string EmployeeCode,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string JobTitle,
    decimal Salary,
    DateTime JoiningDate,
    bool IsActive,
    int DepartmentId,
    string DepartmentName);
