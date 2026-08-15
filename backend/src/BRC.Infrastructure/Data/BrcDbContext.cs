using BRC.Domain.Entities;
using BRC.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BRC.Infrastructure.Data;

public class BrcDbContext : DbContext
{
    public BrcDbContext(DbContextOptions<BrcDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<AcademicGroup> AcademicGroups => Set<AcademicGroup>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SubjectAcademicGroup> SubjectAcademicGroups => Set<SubjectAcademicGroup>();
    public DbSet<TeacherSubjectClass> TeacherSubjectClasses => Set<TeacherSubjectClass>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentAttachment> AssignmentAttachments => Set<AssignmentAttachment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<SubmissionFile> SubmissionFiles => Set<SubmissionFile>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ApplicationSetting> ApplicationSettings => Set<ApplicationSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── User ──────────────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(UserStatus.Active);
        });

        // ─── Student ──────────────────────────────────────────────
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StudentId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.StudentId).IsUnique();
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.RollNumber).HasMaxLength(20);
            entity.Property(e => e.AcademicYear).IsRequired().HasMaxLength(10);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AcademicGroup)
                .WithMany(g => g.Students)
                .HasForeignKey(e => e.AcademicGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── Teacher ──────────────────────────────────────────────
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.EmployeeId).IsUnique();
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.Department).HasMaxLength(100);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── AcademicGroup ──────────────────────────────────────────────
        modelBuilder.Entity<AcademicGroup>(entity =>
        {
            entity.ToTable("AcademicGroups");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // ─── Class ──────────────────────────────────────────────
        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Classes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.AcademicYear).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Section).HasMaxLength(10);

            entity.HasOne(e => e.AcademicGroup)
                .WithMany(g => g.Classes)
                .HasForeignKey(e => e.AcademicGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── Subject ──────────────────────────────────────────────
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subjects");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        // ─── SubjectAcademicGroup ──────────────────────────────────────────────
        modelBuilder.Entity<SubjectAcademicGroup>(entity =>
        {
            entity.ToTable("SubjectAcademicGroups");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SubjectId, e.AcademicGroupId }).IsUnique();

            entity.HasOne(e => e.Subject)
                .WithMany(s => s.SubjectAcademicGroups)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AcademicGroup)
                .WithMany(g => g.SubjectAcademicGroups)
                .HasForeignKey(e => e.AcademicGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── TeacherSubjectClass ──────────────────────────────────────────────
        modelBuilder.Entity<TeacherSubjectClass>(entity =>
        {
            entity.ToTable("TeacherSubjectClasses");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TeacherId, e.SubjectId, e.ClassId }).IsUnique();

            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.TeacherSubjectClasses)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Subject)
                .WithMany(s => s.TeacherSubjectClasses)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Class)
                .WithMany(c => c.TeacherSubjectClasses)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── Assignment ──────────────────────────────────────────────
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.MaximumMarks).IsRequired();
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(AssignmentStatus.Draft);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Deadline);

            entity.HasOne(e => e.Subject)
                .WithMany(s => s.Assignments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Class)
                .WithMany(c => c.Assignments)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Assignments)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── AssignmentAttachment ──────────────────────────────────────────────
        modelBuilder.Entity<AssignmentAttachment>(entity =>
        {
            entity.ToTable("AssignmentAttachments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Attachments)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── Submission ──────────────────────────────────────────────
        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("Submissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TextAnswer).HasMaxLength(10000);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(SubmissionStatus.Draft);
            entity.Property(e => e.Marks).HasColumnType("decimal(5,2)");
            entity.Property(e => e.TeacherFeedback).HasMaxLength(5000);

            // One student can only submit once per assignment
            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Submissions)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── SubmissionFile ──────────────────────────────────────────────
        modelBuilder.Entity<SubmissionFile>(entity =>
        {
            entity.ToTable("SubmissionFiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Submission)
                .WithMany(s => s.Files)
                .HasForeignKey(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── Notification ──────────────────────────────────────────────
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasIndex(e => new { e.UserId, e.IsRead });

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ─── ApplicationSetting ──────────────────────────────────────────────
        modelBuilder.Entity<ApplicationSetting>(entity =>
        {
            entity.ToTable("ApplicationSettings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(500);
        });
    }
}
