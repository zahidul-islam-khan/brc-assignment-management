namespace BRC.Domain.Entities;

public class Class
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., XI Science A
    public Guid AcademicGroupId { get; set; }
    public string AcademicYear { get; set; } = string.Empty; // e.g., 2026
    public string? Section { get; set; } // e.g., A, B
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public AcademicGroup AcademicGroup { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<TeacherSubjectClass> TeacherSubjectClasses { get; set; } = new List<TeacherSubjectClass>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
