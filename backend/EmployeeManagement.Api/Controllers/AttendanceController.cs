using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController, Authorize, Route("api/attendance")]
public class AttendanceController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int? employeeId)
    {
        var start = from ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var end = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var query = db.Attendances.AsNoTracking().Include(a => a.Employee).Where(a => a.Date >= start && a.Date <= end);
        if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId.Value);
        var records = await query.OrderByDescending(a => a.Date).ThenBy(a => a.Employee!.FirstName)
            .Select(a => new AttendanceResponse(a.Id, a.EmployeeId, a.Employee!.FirstName + " " + a.Employee.LastName, a.Date, a.Status, a.Notes)).ToListAsync();
        return Ok(records);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(AttendanceRequest request)
    {
        if (!await db.Employees.AnyAsync(e => e.Id == request.EmployeeId)) return BadRequest(new { message = "Invalid employee." });
        var item = await db.Attendances.SingleOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.Date == request.Date);
        if (item is null) { item = new Attendance { EmployeeId = request.EmployeeId, Date = request.Date }; db.Attendances.Add(item); }
        item.Status = request.Status; item.Notes = request.Notes?.Trim(); await db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Attendances.FindAsync(id); if (item is null) return NotFound();
        db.Attendances.Remove(item); await db.SaveChangesAsync(); return NoContent();
    }
}
