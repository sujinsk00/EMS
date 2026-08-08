using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController, Authorize, Route("api/reports")]
public class ReportsController(ReportService reports) : ControllerBase
{
    [HttpGet("employees/excel")]
    public async Task<IActionResult> EmployeeExcel() => File(await reports.EmployeeExcelAsync(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employee-directory.xlsx");

    [HttpGet("employees/pdf")]
    public async Task<IActionResult> EmployeePdf() => File(await reports.EmployeePdfAsync(), "application/pdf", "employee-directory.pdf");

    [HttpGet("attendance/excel")]
    public async Task<IActionResult> AttendanceExcel([FromQuery] DateOnly from, [FromQuery] DateOnly to) => File(await reports.AttendanceExcelAsync(from, to), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "attendance-report.xlsx");
}
