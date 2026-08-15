namespace BRC.Domain.Entities;

/// <summary>
/// Assigns a teacher to teach a specific subject in a specific class.
/// </summary>
public class TeacherSubjectClass
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Teacher Teacher { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public Class Class { get; set; } = null!;
}
