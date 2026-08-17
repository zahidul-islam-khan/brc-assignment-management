using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Users;
using BRC.Application.Services.Interfaces;
using BRC.Domain.Entities;
using BRC.Domain.Enums;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Application.Services;

public class UserService : IUserService
{
    private readonly BrcDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(BrcDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<UserDto>> GetUsersAsync(PaginationParams pagination, UserFilterParams filters)
    {
        var query = _context.Users
            .Include(u => u.Student).ThenInclude(s => s!.Class)
            .Include(u => u.Student).ThenInclude(s => s!.AcademicGroup)
            .Include(u => u.Teacher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.Role) && Enum.TryParse<UserRole>(filters.Role, true, out var role))
            query = query.Where(u => u.Role == role);

        if (!string.IsNullOrWhiteSpace(filters.Status) && Enum.TryParse<UserStatus>(filters.Status, true, out var status))
            query = query.Where(u => u.Status == status);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<UserDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Student).ThenInclude(s => s!.Class)
            .Include(u => u.Student).ThenInclude(s => s!.AcademicGroup)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email.ToLower().Trim()))
            throw new InvalidOperationException("A user with this email already exists.");

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            throw new InvalidOperationException($"Invalid role: {dto.Role}");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 8),
            Phone = dto.Phone,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        if (role == UserRole.Student)
        {
            if (string.IsNullOrWhiteSpace(dto.StudentId) || dto.AcademicGroupId == null || dto.ClassId == null)
                throw new InvalidOperationException("Student ID, academic group, and class are required for student users.");

            var student = new Student
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                StudentId = dto.StudentId,
                AcademicGroupId = dto.AcademicGroupId.Value,
                ClassId = dto.ClassId.Value,
                RollNumber = dto.RollNumber,
                AcademicYear = dto.AcademicYear ?? DateTime.UtcNow.Year.ToString(),
                CreatedAt = DateTime.UtcNow
            };
            _context.Students.Add(student);
        }
        else if (role == UserRole.Teacher)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
                throw new InvalidOperationException("Employee ID is required for teacher users.");

            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                EmployeeId = dto.EmployeeId,
                Department = dto.Department,
                CreatedAt = DateTime.UtcNow
            };
            _context.Teachers.Add(teacher);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Created user {Email} with role {Role}", user.Email, role);

        return (await GetUserByIdAsync(user.Id))!;
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        // Check email uniqueness if changed
        if (user.Email != dto.Email.ToLower().Trim())
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email.ToLower().Trim() && u.Id != id))
                throw new InvalidOperationException("A user with this email already exists.");
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email.ToLower().Trim();
        user.Phone = dto.Phone;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 8);

        if (user.Student != null)
        {
            if (dto.AcademicGroupId.HasValue) user.Student.AcademicGroupId = dto.AcademicGroupId.Value;
            if (dto.ClassId.HasValue) user.Student.ClassId = dto.ClassId.Value;
            if (dto.RollNumber != null) user.Student.RollNumber = dto.RollNumber;
        }

        if (user.Teacher != null)
        {
            if (dto.Department != null) user.Teacher.Department = dto.Department;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated user {Email}", user.Email);

        return await GetUserByIdAsync(id);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        // Check for dependent data
        if (user.Role == UserRole.Teacher)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == id);
            if (teacher != null)
            {
                var hasAssignments = await _context.Assignments.AnyAsync(a => a.TeacherId == teacher.Id);
                if (hasAssignments)
                    throw new InvalidOperationException("Cannot delete teacher with existing assignments. Deactivate the user instead.");
            }
        }

        if (user.Role == UserRole.Student)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == id);
            if (student != null)
            {
                var hasSubmissions = await _context.Submissions.AnyAsync(s => s.StudentId == student.Id);
                if (hasSubmissions)
                    throw new InvalidOperationException("Cannot delete student with existing submissions. Deactivate the user instead.");
            }
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted user {Email}", user.Email);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, UpdateUserStatusDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        if (!Enum.TryParse<UserStatus>(dto.Status, true, out var status))
            throw new InvalidOperationException($"Invalid status: {dto.Status}");

        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated status for {Email} to {Status}", user.Email, status);
        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString().ToLower(),
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            StudentId = user.Student?.StudentId,
            ClassId = user.Student?.ClassId,
            AcademicGroupId = user.Student?.AcademicGroupId,
            ClassName = user.Student?.Class?.Name,
            GroupName = user.Student?.AcademicGroup?.Name,
            RollNumber = user.Student?.RollNumber,
            EmployeeId = user.Teacher?.EmployeeId,
            Department = user.Teacher?.Department
        };
    }
}
