using System.Text.Json.Serialization;
using System.Text;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Controllers
// ----------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });


// ----------------------------------------------------
// Swagger
// ----------------------------------------------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee Management API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// ----------------------------------------------------
// Database
// ----------------------------------------------------
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));


// ----------------------------------------------------
// Application Services
// ----------------------------------------------------
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ReportService>();


// ----------------------------------------------------
// JWT Authentication
// ----------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT key is missing from appsettings.json.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "JWT issuer is missing from appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT audience is missing from appsettings.json.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                )
        };
    });

builder.Services.AddAuthorization();


// ----------------------------------------------------
// CORS
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ----------------------------------------------------
// Build Application
// ----------------------------------------------------
var app = builder.Build();


// ----------------------------------------------------
// Database Initialization + Seed
// ----------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();

    await SeedAsync(db);
}


// ----------------------------------------------------
// Middleware
// ----------------------------------------------------
app.UseCors("Frontend");

app.UseAuthentication();

app.UseAuthorization();


// ----------------------------------------------------
// Swagger
// ----------------------------------------------------
app.UseSwagger();

app.UseSwaggerUI();


// ----------------------------------------------------
// Controllers
// ----------------------------------------------------
app.MapControllers();


// ----------------------------------------------------
// Health Check
// ----------------------------------------------------
app.MapGet("/api/health", () =>
    Results.Ok(new
    {
        status = "ok",
        timestamp = DateTime.UtcNow
    }));


// ----------------------------------------------------
// Run
// ----------------------------------------------------
app.Run();


// ====================================================
// Database Seed
// ====================================================
static async Task SeedAsync(AppDbContext db)
{
    // ------------------------------------------------
    // Departments
    // ------------------------------------------------
    if (!await db.Departments.AnyAsync())
    {
        db.Departments.AddRange(
            new Department
            {
                Name = "Engineering",
                Description = "Software and technology"
            },

            new Department
            {
                Name = "Human Resources",
                Description = "People operations"
            },

            new Department
            {
                Name = "Finance",
                Description = "Finance and accounting"
            },

            new Department
            {
                Name = "Sales",
                Description = "Sales and business development"
            }
        );

        await db.SaveChangesAsync();
    }


    // ------------------------------------------------
    // Admin User
    // ------------------------------------------------
    if (!await db.Users.AnyAsync())
    {
        var hasher =
            new Microsoft.AspNetCore.Identity.PasswordHasher<AppUser>();

        var admin = new AppUser
        {
            Username = "admin",
            Role = "Admin"
        };

        admin.PasswordHash =
            hasher.HashPassword(
                admin,
                "Admin@123"
            );

        db.Users.Add(admin);

        await db.SaveChangesAsync();
    }
}