namespace BRC.Application.DTOs.Submissions;

public class SubmissionDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentRollNumber { get; set; }
    public string? TextAnswer { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public decimal? Marks { get; set; }
    public int MaximumMarks { get; set; }
    public string? TeacherFeedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public List<SubmissionFileDto> Files { get; set; } = new();
}

public class CreateSubmissionDto
{
    public string? TextAnswer { get; set; }
}

public class UpdateSubmissionDto
{
    public string? TextAnswer { get; set; }
}

public class GradeSubmissionDto
{
    public decimal Marks { get; set; }
    public string? Feedback { get; set; }
    public string? Status { get; set; } // Graded or Returned
}

public class SubmissionFileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class SubmissionFilterParams
{
    public string? Search { get; set; }
    public Guid? AssignmentId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? StudentId { get; set; }
    public string? Status { get; set; }
}
