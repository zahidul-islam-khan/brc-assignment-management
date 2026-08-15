namespace BRC.Application.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = null!;
}

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Name => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Student-specific
    public string? StudentId { get; set; }
    public string? ClassName { get; set; }
    public string? GroupName { get; set; }

    // Teacher-specific
    public string? EmployeeId { get; set; }
    public string? Department { get; set; }
}
