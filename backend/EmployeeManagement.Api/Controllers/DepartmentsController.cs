using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController, Authorize, Route("api/departments")]
public class DepartmentsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await db.Departments.AsNoTracking().OrderBy(d => d.Name)
        .Select(d => new { d.Id, d.Name, d.Description, EmployeeCount = d.Employees.Count }).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var d = await db.Departments.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new { x.Id, x.Name, x.Description, EmployeeCount = x.Employees.Count }).SingleOrDefaultAsync();
        return d is null ? NotFound() : Ok(d);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DepartmentRequest request)
    {
        if (await db.Departments.AnyAsync(d => d.Name == request.Name)) return Conflict(new { message = "Department already exists." });
        var d = new Department { Name = request.Name.Trim(), Description = request.Description?.Trim() };
        db.Departments.Add(d); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = d.Id }, d);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DepartmentRequest request)
    {
        var d = await db.Departments.FindAsync(id);
        if (d is null) return NotFound();
        if (await db.Departments.AnyAsync(x => x.Id != id && x.Name == request.Name)) return Conflict(new { message = "Department already exists." });
        d.Name = request.Name.Trim(); d.Description = request.Description?.Trim(); await db.SaveChangesAsync(); return Ok(d);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var d = await db.Departments.Include(x => x.Employees).SingleOrDefaultAsync(x => x.Id == id);
        if (d is null) return NotFound();
        if (d.Employees.Any()) return BadRequest(new { message = "Cannot delete a department that has employees." });
        db.Departments.Remove(d); await db.SaveChangesAsync(); return NoContent();
    }
}

public class DepartmentRequest
{
    [Required, MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
}
