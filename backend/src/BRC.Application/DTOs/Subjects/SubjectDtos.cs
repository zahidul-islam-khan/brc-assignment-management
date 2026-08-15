namespace BRC.Application.DTOs.Subjects;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsActive { get; set; }
    public List<string> AcademicGroups { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateSubjectDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public List<Guid> AcademicGroupIds { get; set; } = new();
}

public class UpdateSubjectDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsActive { get; set; }
    public List<Guid> AcademicGroupIds { get; set; } = new();
}

public class AcademicGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int ClassCount { get; set; }
    public int StudentCount { get; set; }
}
