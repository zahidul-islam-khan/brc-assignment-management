namespace BRC.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Name => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Student-specific
    public string? StudentId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? AcademicGroupId { get; set; }
    public string? ClassName { get; set; }
    public string? GroupName { get; set; }
    public string? RollNumber { get; set; }

    // Teacher-specific
    public string? EmployeeId { get; set; }
    public string? Department { get; set; }
}

public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;

    // Student-specific
    public string? StudentId { get; set; }
    public Guid? AcademicGroupId { get; set; }
    public Guid? ClassId { get; set; }
    public string? RollNumber { get; set; }
    public string? AcademicYear { get; set; }

    // Teacher-specific
    public string? EmployeeId { get; set; }
    public string? Department { get; set; }
}

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Password { get; set; }

    // Student-specific
    public Guid? AcademicGroupId { get; set; }
    public Guid? ClassId { get; set; }
    public string? RollNumber { get; set; }

    // Teacher-specific
    public string? Department { get; set; }
}

public class UpdateUserStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class UserFilterParams
{
    public string? Search { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; }
    public string? GroupName { get; set; }
}
