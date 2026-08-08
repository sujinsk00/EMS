using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController, Authorize, Route("api/employees")]
public class EmployeesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int? departmentId, [FromQuery] bool? active)
    {
        var query = db.Employees.AsNoTracking().Include(e => e.Department).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.EmployeeCode.Contains(search) || e.FirstName.Contains(search) || e.LastName.Contains(search) || e.Email.Contains(search));
        if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId.Value);
        if (active.HasValue) query = query.Where(e => e.IsActive == active.Value);
        var data = await query.OrderBy(e => e.FirstName).Select(Map).ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await db.Employees.AsNoTracking().Include(x => x.Department).SingleOrDefaultAsync(x => x.Id == id);
        return e is null ? NotFound() : Ok(ToResponse(e));
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeCreateRequest request)
    {
        var validation = await ValidateUnique(request.EmployeeCode, request.Email, null, request.DepartmentId);
        if (validation is not null) return validation;
        var e = new Employee(); Apply(e, request); db.Employees.Add(e); await db.SaveChangesAsync();
        await db.Entry(e).Reference(x => x.Department).LoadAsync();
        return CreatedAtAction(nameof(Get), new { id = e.Id }, ToResponse(e));
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate(List<EmployeeCreateRequest> requests)
    {
        if (requests.Count == 0) return BadRequest(new { message = "At least one employee is required." });
        if (requests.Count > 500) return BadRequest(new { message = "Maximum 500 employees per request." });
        var codes = requests.Select(x => x.EmployeeCode.Trim().ToUpper()).ToList();
        var emails = requests.Select(x => x.Email.Trim().ToLower()).ToList();
        if (codes.Count != codes.Distinct().Count() || emails.Count != emails.Distinct().Count()) return BadRequest(new { message = "Duplicate employee code or email in request." });
        var existing = await db.Employees.Where(e => codes.Contains(e.EmployeeCode.ToUpper()) || emails.Contains(e.Email.ToLower())).AnyAsync();
        if (existing) return Conflict(new { message = "One or more employee codes/emails already exist." });
        foreach (var request in requests)
        {
            var e = new Employee(); Apply(e, request); db.Employees.Add(e);
        }
        await db.SaveChangesAsync();
        return Ok(new { message = $"{requests.Count} employees created successfully." });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeUpdateRequest request)
    {
        var e = await db.Employees.FindAsync(id); if (e is null) return NotFound();
        var validation = await ValidateUnique(request.EmployeeCode, request.Email, id, request.DepartmentId);
        if (validation is not null) return validation;
        Apply(e, request); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync();
        await db.Entry(e).Reference(x => x.Department).LoadAsync(); return Ok(ToResponse(e));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Employees.FindAsync(id); if (e is null) return NotFound();
        db.Employees.Remove(e); await db.SaveChangesAsync(); return NoContent();
    }

    private async Task<IActionResult?> ValidateUnique(string code, string email, int? ignoreId, int departmentId)
    {
        if (!await db.Departments.AnyAsync(d => d.Id == departmentId)) return BadRequest(new { message = "Invalid department." });
        var duplicateCode = await db.Employees.AnyAsync(e => e.EmployeeCode == code.Trim() && e.Id != ignoreId);
        if (duplicateCode) return Conflict(new { message = "Employee code already exists." });
        var duplicateEmail = await db.Employees.AnyAsync(e => e.Email == email.Trim() && e.Id != ignoreId);
        if (duplicateEmail) return Conflict(new { message = "Email already exists." });
        return null;
    }

    private static void Apply(Employee e, EmployeeCreateRequest r)
    {
        e.EmployeeCode = r.EmployeeCode.Trim(); e.FirstName = r.FirstName.Trim(); e.LastName = r.LastName.Trim();
        e.Email = r.Email.Trim(); e.Phone = r.Phone.Trim(); e.JobTitle = r.JobTitle.Trim(); e.Salary = r.Salary;
        e.JoiningDate = r.JoiningDate; e.DepartmentId = r.DepartmentId; e.IsActive = r.IsActive;
    }

    private static EmployeeResponse ToResponse(Employee e) => new(e.Id, e.EmployeeCode, e.FirstName, e.LastName, e.Email, e.Phone, e.JobTitle, e.Salary, e.JoiningDate, e.IsActive, e.DepartmentId, e.Department?.Name ?? "");
    private static System.Linq.Expressions.Expression<Func<Employee, EmployeeResponse>> Map => e => new(e.Id, e.EmployeeCode, e.FirstName, e.LastName, e.Email, e.Phone, e.JobTitle, e.Salary, e.JoiningDate, e.IsActive, e.DepartmentId, e.Department!.Name);
}
