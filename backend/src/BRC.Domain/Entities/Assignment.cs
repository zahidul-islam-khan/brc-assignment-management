using BRC.Domain.Enums;

namespace BRC.Domain.Entities;

public class Assignment
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId { get; set; }
    public Guid TeacherId { get; set; }
    public DateTime Deadline { get; set; }
    public int MaximumMarks { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Navigation properties
    public Subject Subject { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public ICollection<AssignmentAttachment> Attachments { get; set; } = new List<AssignmentAttachment>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
