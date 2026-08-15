namespace BRC.Domain.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g., PHY, CHE
    public string Name { get; set; } = string.Empty; // e.g., Physics, Chemistry
    public int Credits { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<SubjectAcademicGroup> SubjectAcademicGroups { get; set; } = new List<SubjectAcademicGroup>();
    public ICollection<TeacherSubjectClass> TeacherSubjectClasses { get; set; } = new List<TeacherSubjectClass>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
