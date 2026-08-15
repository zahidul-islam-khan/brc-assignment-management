namespace BRC.Application.DTOs.Assignments;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaximumMarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SubmissionCount { get; set; }
    public int TotalStudents { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
}

public class CreateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId { get; set; }
    public DateTime Deadline { get; set; }
    public int MaximumMarks { get; set; }
}

public class UpdateAssignmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public int MaximumMarks { get; set; }
}

public class AttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class AssignmentFilterParams
{
    public string? Search { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? TeacherId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Assignment as seen by a student, with their submission status.
/// </summary>
public class StudentAssignmentDto : AssignmentDto
{
    public string? SubmissionStatus { get; set; }
    public decimal? MyMarks { get; set; }
    public Guid? SubmissionId { get; set; }
}
