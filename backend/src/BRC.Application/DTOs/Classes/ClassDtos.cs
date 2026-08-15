namespace BRC.Application.DTOs.Classes;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid AcademicGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? Section { get; set; }
    public bool IsActive { get; set; }
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
    public Guid AcademicGroupId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string? Section { get; set; }
}

public class UpdateClassDto
{
    public string Name { get; set; } = string.Empty;
    public Guid AcademicGroupId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string? Section { get; set; }
    public bool IsActive { get; set; }
}
