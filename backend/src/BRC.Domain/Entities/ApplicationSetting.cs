namespace BRC.Domain.Entities;

/// <summary>
/// Key-value store for application-wide settings like AllowResubmission, AllowLateSubmission.
/// </summary>
public class ApplicationSetting
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
