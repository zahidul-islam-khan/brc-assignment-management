namespace BRC.Application.DTOs.Dashboard;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalSubmissions { get; set; }
    public int PendingSubmissions { get; set; }
    public List<RecentActivityDto> RecentActivity { get; set; } = new();
}

public class TeacherDashboardDto
{
    public int TotalAssignments { get; set; }
    public int PublishedAssignments { get; set; }
    public int DraftAssignments { get; set; }
    public int TotalSubmissions { get; set; }
    public int PendingGrading { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }
    public List<RecentActivityDto> RecentActivity { get; set; } = new();
}

public class StudentDashboardDto
{
    public int TotalAssignments { get; set; }
    public int SubmittedCount { get; set; }
    public int PendingCount { get; set; }
    public int GradedCount { get; set; }
    public int OverdueCount { get; set; }
    public decimal? AverageMarks { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
}

public class RecentActivityDto
{
    public string Who { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}
