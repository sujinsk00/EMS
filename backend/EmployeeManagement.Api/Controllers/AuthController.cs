using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Services;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController, Route("api/auth")]
public class AuthController(AppDbContext db, JwtService jwt) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user is null) return Unauthorized(new { message = "Invalid username or password." });
        var result = new PasswordHasher<AppUser>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized(new { message = "Invalid username or password." });
        var token = jwt.CreateToken(user);
        return Ok(new LoginResponse(token.Token, user.Username, user.Role, token.ExpiresAt));
    }
}
