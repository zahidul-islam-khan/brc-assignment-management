namespace BRC.Domain.Entities;

public class Teacher
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty; // e.g., BRC-T-014
    public string? Department { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<TeacherSubjectClass> TeacherSubjectClasses { get; set; } = new List<TeacherSubjectClass>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
