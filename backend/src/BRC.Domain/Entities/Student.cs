namespace BRC.Domain.Entities;

public class Student
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StudentId { get; set; } = string.Empty; // e.g., BRC-2026-001
    public Guid AcademicGroupId { get; set; }
    public Guid ClassId { get; set; }
    public string? RollNumber { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public AcademicGroup AcademicGroup { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
