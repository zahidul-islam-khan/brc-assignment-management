namespace BRC.Domain.Entities;

public class AssignmentAttachment
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
}
