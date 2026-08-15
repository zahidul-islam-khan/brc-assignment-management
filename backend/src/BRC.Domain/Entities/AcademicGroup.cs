namespace BRC.Domain.Entities;

public class AcademicGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Science, Business Studies, Humanities
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<SubjectAcademicGroup> SubjectAcademicGroups { get; set; } = new List<SubjectAcademicGroup>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
