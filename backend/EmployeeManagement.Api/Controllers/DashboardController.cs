using EmployeeManagement.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // Total employees
        var total = await _db.Employees
            .AsNoTracking()
            .CountAsync();

        // Active employees
        var active = await _db.Employees
            .AsNoTracking()
            .CountAsync(e => e.IsActive);

        // Total active employee salary
        var salary = await _db.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .SumAsync(e => (decimal?)e.Salary) ?? 0;

        // Employees hired this month
        var now = DateTime.UtcNow;

        var monthStart = new DateTime(
            now.Year,
            now.Month,
            1
        );

        var hiredThisMonth = await _db.Employees
            .AsNoTracking()
            .CountAsync(e => e.JoiningDate >= monthStart);

        // Today's date
        var today = DateOnly.FromDateTime(now);

        // Present today
        var presentToday = await _db.Attendances
            .AsNoTracking()
            .CountAsync(a =>
                a.Date == today &&
                a.Status == Models.AttendanceStatus.Present);

        // Absent today
        var absentToday = await _db.Attendances
            .AsNoTracking()
            .CountAsync(a =>
                a.Date == today &&
                a.Status == Models.AttendanceStatus.Absent);

        // Department employee counts
        var departments = await _db.Departments
            .AsNoTracking()
            .Select(d => new
            {
                d.Name,
                EmployeeCount = d.Employees.Count()
            })
            .OrderByDescending(x => x.EmployeeCount)
            .ToListAsync();

        // IMPORTANT:
        // Do not format Year/Month inside the EF expression.
        // First get numeric values from MySQL.
        var hiringTrendRaw = await _db.Employees
            .AsNoTracking()
            .GroupBy(e => new
            {
                Year = e.JoiningDate.Year,
                Month = e.JoiningDate.Month
            })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .Take(12)
            .ToListAsync();

        // Format after the database query has completed.
        var hiringTrend = hiringTrendRaw
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .Select(x => new
            {
                month = $"{x.Year}-{x.Month:D2}",
                count = x.Count
            })
            .ToList();

        // Attendance during the last 30 days
        var attendanceStartDate =
            DateOnly.FromDateTime(now.AddDays(-30));

        var attendance = await _db.Attendances
            .AsNoTracking()
            .Where(a => a.Date >= attendanceStartDate)
            .GroupBy(a => a.Status)
            .Select(g => new
            {
                status = g.Key.ToString(),
                count = g.Count()
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            active,
            salary,
            hiredThisMonth,
            presentToday,
            absentToday,
            departments,
            hiringTrend,
            attendance
        });
    }
}
