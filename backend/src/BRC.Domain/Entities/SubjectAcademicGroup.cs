namespace BRC.Domain.Entities;

/// <summary>
/// Many-to-many junction between Subject and AcademicGroup.
/// A subject like Bangla or English can belong to multiple groups.
/// </summary>
public class SubjectAcademicGroup
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public Guid AcademicGroupId { get; set; }

    // Navigation properties
    public Subject Subject { get; set; } = null!;
    public AcademicGroup AcademicGroup { get; set; } = null!;
}
