namespace BRC.Application.DTOs.TeacherAssignments;

public class TeacherAssignmentDto
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}

public class CreateTeacherAssignmentDto
{
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId { get; set; }
}
