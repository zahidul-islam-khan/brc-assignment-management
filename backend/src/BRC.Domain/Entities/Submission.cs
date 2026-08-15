using BRC.Domain.Enums;

namespace BRC.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string? TextAnswer { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal? Marks { get; set; }
    public string? TeacherFeedback { get; set; }
    public DateTime? GradedAt { get; set; }

    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();
}
