using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BRC.Application.DTOs.Auth;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Enums;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BRC.Application.Services;

public class AuthService : IAuthService
{
    private readonly BrcDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(BrcDbContext context, IConfiguration config, ILogger<AuthService> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.Student).ThenInclude(s => s!.Class)
            .Include(u => u.Student).ThenInclude(s => s!.AcademicGroup)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());

        if (user == null)
        {
            _logger.LogWarning("Login failed: unknown email {Email}", request.Email);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: invalid password for {Email}", request.Email);
            return null;
        }

        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Login failed: user {Email} is {Status}", request.Email, user.Status);
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:ExpirationMinutes"] ?? "480"));

        _logger.LogInformation("User {Email} logged in successfully as {Role}", user.Email, user.Role);

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString().ToLower(),
            Status = user.Status.ToString(),
        };

        if (user.Student != null)
        {
            userInfo.StudentId = user.Student.StudentId;
            userInfo.ClassName = user.Student.Class?.Name;
            userInfo.GroupName = user.Student.AcademicGroup?.Name;
        }
        if (user.Teacher != null)
        {
            userInfo.EmployeeId = user.Teacher.EmployeeId;
            userInfo.Department = user.Teacher.Department;
        }

        return new LoginResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiration,
            User = userInfo
        };
    }

    private string GenerateJwtToken(Domain.Entities.User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpirationMinutes"] ?? "480")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
