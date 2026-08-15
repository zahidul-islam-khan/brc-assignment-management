using BRC.Domain.Entities;
using BRC.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRC.Infrastructure.Data.Seed;

public class DataSeeder
{
    private readonly BrcDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(BrcDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Seeding database...");

        // ─── Academic Groups ───────────────────────────────────────
        var science = new AcademicGroup { Id = Guid.NewGuid(), Name = "Science", Description = "Higher Secondary Certificate — Science stream" };
        var business = new AcademicGroup { Id = Guid.NewGuid(), Name = "Business Studies", Description = "Higher Secondary Certificate — Business Studies stream" };
        var humanities = new AcademicGroup { Id = Guid.NewGuid(), Name = "Humanities", Description = "Higher Secondary Certificate — Humanities stream" };

        _context.AcademicGroups.AddRange(science, business, humanities);

        // ─── Classes ───────────────────────────────────────
        var xiSciA = new Class { Id = Guid.NewGuid(), Name = "XI Science A", AcademicGroupId = science.Id, AcademicYear = "2026", Section = "A" };
        var xiSciB = new Class { Id = Guid.NewGuid(), Name = "XI Science B", AcademicGroupId = science.Id, AcademicYear = "2026", Section = "B" };
        var xiiSciA = new Class { Id = Guid.NewGuid(), Name = "XII Science A", AcademicGroupId = science.Id, AcademicYear = "2026", Section = "A" };
        var xiBusA = new Class { Id = Guid.NewGuid(), Name = "XI Business Studies A", AcademicGroupId = business.Id, AcademicYear = "2026", Section = "A" };
        var xiiBusA = new Class { Id = Guid.NewGuid(), Name = "XII Business Studies A", AcademicGroupId = business.Id, AcademicYear = "2026", Section = "A", IsActive = false };
        var xiHumA = new Class { Id = Guid.NewGuid(), Name = "XI Humanities A", AcademicGroupId = humanities.Id, AcademicYear = "2026", Section = "A" };
        var xiiHumA = new Class { Id = Guid.NewGuid(), Name = "XII Humanities A", AcademicGroupId = humanities.Id, AcademicYear = "2026", Section = "A" };

        _context.Classes.AddRange(xiSciA, xiSciB, xiiSciA, xiBusA, xiiBusA, xiHumA, xiiHumA);

        // ─── Subjects ───────────────────────────────────────
        var bangla = new Subject { Id = Guid.NewGuid(), Code = "BAN", Name = "Bangla", Credits = 4 };
        var english = new Subject { Id = Guid.NewGuid(), Code = "ENG", Name = "English", Credits = 4 };
        var ict = new Subject { Id = Guid.NewGuid(), Code = "ICT", Name = "Information and Communication Technology", Credits = 2 };
        var physics = new Subject { Id = Guid.NewGuid(), Code = "PHY", Name = "Physics", Credits = 4 };
        var chemistry = new Subject { Id = Guid.NewGuid(), Code = "CHE", Name = "Chemistry", Credits = 4 };
        var higherMath = new Subject { Id = Guid.NewGuid(), Code = "MAT", Name = "Higher Mathematics", Credits = 4 };
        var biology = new Subject { Id = Guid.NewGuid(), Code = "BIO", Name = "Biology", Credits = 4 };
        var accounting = new Subject { Id = Guid.NewGuid(), Code = "ACC", Name = "Accounting", Credits = 4 };
        var finance = new Subject { Id = Guid.NewGuid(), Code = "FBI", Name = "Finance, Banking and Insurance", Credits = 4 };
        var bom = new Subject { Id = Guid.NewGuid(), Code = "BOM", Name = "Business Organization and Management", Credits = 4 };
        var pmm = new Subject { Id = Guid.NewGuid(), Code = "PMM", Name = "Production Management and Marketing", Credits = 4 };
        var economics = new Subject { Id = Guid.NewGuid(), Code = "ECO", Name = "Economics", Credits = 4 };
        var civics = new Subject { Id = Guid.NewGuid(), Code = "CIV", Name = "Civics and Good Governance", Credits = 4 };
        var history = new Subject { Id = Guid.NewGuid(), Code = "HIS", Name = "History", Credits = 4, IsActive = false };
        var geography = new Subject { Id = Guid.NewGuid(), Code = "GEO", Name = "Geography", Credits = 4 };
        var logic = new Subject { Id = Guid.NewGuid(), Code = "LOG", Name = "Logic", Credits = 4 };
        var sociology = new Subject { Id = Guid.NewGuid(), Code = "SOC", Name = "Sociology", Credits = 4 };

        _context.Subjects.AddRange(bangla, english, ict, physics, chemistry, higherMath, biology,
            accounting, finance, bom, pmm, economics, civics, history, geography, logic, sociology);

        // ─── Subject ↔ AcademicGroup associations ───────────────────────────────────────
        // Common subjects across all groups
        var sagList = new List<SubjectAcademicGroup>();
        foreach (var group in new[] { science, business, humanities })
        {
            sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = bangla.Id, AcademicGroupId = group.Id });
            sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = english.Id, AcademicGroupId = group.Id });
            sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = ict.Id, AcademicGroupId = group.Id });
        }
        // Science-specific
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = physics.Id, AcademicGroupId = science.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = chemistry.Id, AcademicGroupId = science.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = higherMath.Id, AcademicGroupId = science.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = biology.Id, AcademicGroupId = science.Id });
        // Business-specific
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = accounting.Id, AcademicGroupId = business.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = finance.Id, AcademicGroupId = business.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = bom.Id, AcademicGroupId = business.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = pmm.Id, AcademicGroupId = business.Id });
        // Humanities-specific
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = economics.Id, AcademicGroupId = humanities.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = civics.Id, AcademicGroupId = humanities.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = history.Id, AcademicGroupId = humanities.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = geography.Id, AcademicGroupId = humanities.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = logic.Id, AcademicGroupId = humanities.Id });
        sagList.Add(new SubjectAcademicGroup { Id = Guid.NewGuid(), SubjectId = sociology.Id, AcademicGroupId = humanities.Id });

        _context.SubjectAcademicGroups.AddRange(sagList);

        // ─── Users ───────────────────────────────────────
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password@123");

        // Admin
        var adminUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Tanvir", LastName = "Ahmed",
            Email = "admin@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000001", Role = UserRole.Admin,
            CreatedAt = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        // Teachers
        var nusratUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Nusrat", LastName = "Jahan",
            Email = "nusrat.jahan@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000012", Role = UserRole.Teacher,
            CreatedAt = new DateTime(2024, 2, 3, 0, 0, 0, DateTimeKind.Utc)
        };
        var rakibUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Md. Rakib", LastName = "Hasan",
            Email = "rakib.hasan@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000013", Role = UserRole.Teacher,
            CreatedAt = new DateTime(2024, 2, 5, 0, 0, 0, DateTimeKind.Utc)
        };
        var farhanaUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Farhana", LastName = "Akter",
            Email = "farhana.akter@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000014", Role = UserRole.Teacher,
            CreatedAt = new DateTime(2024, 2, 9, 0, 0, 0, DateTimeKind.Utc)
        };
        var abdulUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Md. Abdul", LastName = "Karim",
            Email = "abdul.karim@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000011", Role = UserRole.Teacher,
            CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        var saifulUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Saiful", LastName = "Islam",
            Email = "saiful.islam@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000015", Role = UserRole.Teacher, Status = UserStatus.Inactive,
            CreatedAt = new DateTime(2024, 2, 11, 0, 0, 0, DateTimeKind.Utc)
        };
        var sharminUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Sharmin", LastName = "Sultana",
            Email = "sharmin.sultana@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000016", Role = UserRole.Teacher,
            CreatedAt = new DateTime(2024, 2, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        // Students
        var fahimUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Fahim", LastName = "Rahman",
            Email = "fahim.rahman@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000112", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 18, 0, 0, 0, DateTimeKind.Utc)
        };
        var sadiaUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Sadia", LastName = "Islam",
            Email = "sadia.islam@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000113", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 18, 0, 0, 0, DateTimeKind.Utc)
        };
        var arifUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Arif", LastName = "Hossain",
            Email = "arif.hossain@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000114", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 18, 0, 0, 0, DateTimeKind.Utc)
        };
        var mehediUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Mehedi", LastName = "Hasan",
            Email = "mehedi.hasan@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000115", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 19, 0, 0, 0, DateTimeKind.Utc)
        };
        var nusratTUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Nusrat", LastName = "Tasnim",
            Email = "nusrat.tasnim@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000116", Role = UserRole.Student, Status = UserStatus.Suspended,
            CreatedAt = new DateTime(2024, 2, 19, 0, 0, 0, DateTimeKind.Utc)
        };
        var tasmiaUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Tasmia", LastName = "Akter",
            Email = "tasmia.akter@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000117", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc)
        };
        var mahmudulUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Mahmudul", LastName = "Hasan",
            Email = "mahmudul.hasan@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000118", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc)
        };
        var jannatulUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Jannatul", LastName = "Ferdous",
            Email = "jannatul.ferdous@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000119", Role = UserRole.Student, Status = UserStatus.Inactive,
            CreatedAt = new DateTime(2024, 2, 21, 0, 0, 0, DateTimeKind.Utc)
        };
        var sumaiyaUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Sumaiya", LastName = "Akter",
            Email = "sumaiya.akter@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000120", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 21, 0, 0, 0, DateTimeKind.Utc)
        };
        var rafiulUser = new User
        {
            Id = Guid.NewGuid(), FirstName = "Rafiul", LastName = "Islam",
            Email = "rafiul.islam@brc.edu.bd", PasswordHash = passwordHash,
            Phone = "+880 1711-000121", Role = UserRole.Student,
            CreatedAt = new DateTime(2024, 2, 22, 0, 0, 0, DateTimeKind.Utc)
        };

        _context.Users.AddRange(adminUser, nusratUser, rakibUser, farhanaUser, abdulUser, saifulUser, sharminUser,
            fahimUser, sadiaUser, arifUser, mehediUser, nusratTUser, tasmiaUser, mahmudulUser, jannatulUser, sumaiyaUser, rafiulUser);

        // ─── Teacher profiles ───────────────────────────────────────
        var nusrat = new Teacher { Id = Guid.NewGuid(), UserId = nusratUser.Id, EmployeeId = "BRC-T-014", Department = "Science" };
        var rakib = new Teacher { Id = Guid.NewGuid(), UserId = rakibUser.Id, EmployeeId = "BRC-T-015", Department = "Science" };
        var farhana = new Teacher { Id = Guid.NewGuid(), UserId = farhanaUser.Id, EmployeeId = "BRC-T-016", Department = "Business Studies" };
        var abdul = new Teacher { Id = Guid.NewGuid(), UserId = abdulUser.Id, EmployeeId = "BRC-T-011", Department = "Science" };
        var saiful = new Teacher { Id = Guid.NewGuid(), UserId = saifulUser.Id, EmployeeId = "BRC-T-017", Department = "Business Studies" };
        var sharmin = new Teacher { Id = Guid.NewGuid(), UserId = sharminUser.Id, EmployeeId = "BRC-T-018", Department = "Humanities" };

        _context.Teachers.AddRange(nusrat, rakib, farhana, abdul, saiful, sharmin);

        // ─── Student profiles ───────────────────────────────────────
        var fahim = new Student { Id = Guid.NewGuid(), UserId = fahimUser.Id, StudentId = "BRC-2026-001", AcademicGroupId = science.Id, ClassId = xiSciA.Id, RollNumber = "014", AcademicYear = "2026" };
        var sadia = new Student { Id = Guid.NewGuid(), UserId = sadiaUser.Id, StudentId = "BRC-2026-002", AcademicGroupId = science.Id, ClassId = xiSciA.Id, RollNumber = "015", AcademicYear = "2026" };
        var arif = new Student { Id = Guid.NewGuid(), UserId = arifUser.Id, StudentId = "BRC-2026-003", AcademicGroupId = science.Id, ClassId = xiSciA.Id, RollNumber = "016", AcademicYear = "2026" };
        var mehedi = new Student { Id = Guid.NewGuid(), UserId = mehediUser.Id, StudentId = "BRC-2026-004", AcademicGroupId = science.Id, ClassId = xiSciB.Id, RollNumber = "041", AcademicYear = "2026" };
        var nusratT = new Student { Id = Guid.NewGuid(), UserId = nusratTUser.Id, StudentId = "BRC-2025-008", AcademicGroupId = science.Id, ClassId = xiiSciA.Id, RollNumber = "008", AcademicYear = "2025" };
        var tasmia = new Student { Id = Guid.NewGuid(), UserId = tasmiaUser.Id, StudentId = "BRC-2026-005", AcademicGroupId = business.Id, ClassId = xiBusA.Id, RollNumber = "006", AcademicYear = "2026" };
        var mahmudul = new Student { Id = Guid.NewGuid(), UserId = mahmudulUser.Id, StudentId = "BRC-2026-006", AcademicGroupId = business.Id, ClassId = xiBusA.Id, RollNumber = "007", AcademicYear = "2026" };
        var jannatul = new Student { Id = Guid.NewGuid(), UserId = jannatulUser.Id, StudentId = "BRC-2025-003", AcademicGroupId = business.Id, ClassId = xiiBusA.Id, RollNumber = "003", AcademicYear = "2025" };
        var sumaiya = new Student { Id = Guid.NewGuid(), UserId = sumaiyaUser.Id, StudentId = "BRC-2026-007", AcademicGroupId = humanities.Id, ClassId = xiHumA.Id, RollNumber = "004", AcademicYear = "2026" };
        var rafiul = new Student { Id = Guid.NewGuid(), UserId = rafiulUser.Id, StudentId = "BRC-2026-008", AcademicGroupId = humanities.Id, ClassId = xiHumA.Id, RollNumber = "005", AcademicYear = "2026" };

        _context.Students.AddRange(fahim, sadia, arif, mehedi, nusratT, tasmia, mahmudul, jannatul, sumaiya, rafiul);

        // ─── Teacher-Subject-Class assignments ───────────────────────────────────────
        var tscList = new List<TeacherSubjectClass>
        {
            // Abdul Karim teaches Physics in XI Science A and Biology in XI Science B
            new() { Id = Guid.NewGuid(), TeacherId = abdul.Id, SubjectId = physics.Id, ClassId = xiSciA.Id },
            new() { Id = Guid.NewGuid(), TeacherId = abdul.Id, SubjectId = biology.Id, ClassId = xiSciB.Id },
            // Nusrat Jahan teaches Higher Math and ICT in XI Science A
            new() { Id = Guid.NewGuid(), TeacherId = nusrat.Id, SubjectId = higherMath.Id, ClassId = xiSciA.Id },
            new() { Id = Guid.NewGuid(), TeacherId = nusrat.Id, SubjectId = ict.Id, ClassId = xiSciA.Id },
            // Rakib Hasan teaches Chemistry in XI Science A
            new() { Id = Guid.NewGuid(), TeacherId = rakib.Id, SubjectId = chemistry.Id, ClassId = xiSciA.Id },
            // Farhana Akter teaches Accounting in XI Business A and BOM in XII Business A
            new() { Id = Guid.NewGuid(), TeacherId = farhana.Id, SubjectId = accounting.Id, ClassId = xiBusA.Id },
            new() { Id = Guid.NewGuid(), TeacherId = farhana.Id, SubjectId = bom.Id, ClassId = xiiBusA.Id },
            // Saiful Islam teaches Finance in XI Business A
            new() { Id = Guid.NewGuid(), TeacherId = saiful.Id, SubjectId = finance.Id, ClassId = xiBusA.Id },
            // Sharmin Sultana teaches Economics and Civics in XI Humanities A, History in XII Humanities A
            new() { Id = Guid.NewGuid(), TeacherId = sharmin.Id, SubjectId = economics.Id, ClassId = xiHumA.Id },
            new() { Id = Guid.NewGuid(), TeacherId = sharmin.Id, SubjectId = civics.Id, ClassId = xiHumA.Id },
            new() { Id = Guid.NewGuid(), TeacherId = sharmin.Id, SubjectId = history.Id, ClassId = xiiHumA.Id },
        };
        _context.TeacherSubjectClasses.AddRange(tscList);

        // ─── Assignments ───────────────────────────────────────
        var a1 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Newton's Laws of Motion — Assignment 01",
            Description = "State and explain Newton's three laws of motion with real-life examples. Solve the attached numerical problems on force, mass and acceleration, showing all steps.",
            SubjectId = physics.Id, ClassId = xiSciA.Id, TeacherId = abdul.Id,
            Deadline = new DateTime(2026, 8, 18, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 20, Status = AssignmentStatus.Published,
            CreatedAt = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc)
        };
        var a2 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Limits and Continuity — Problem Set",
            Description = "Evaluate the given limits and determine the continuity of each function at the stated points.",
            SubjectId = higherMath.Id, ClassId = xiSciA.Id, TeacherId = nusrat.Id,
            Deadline = new DateTime(2026, 8, 24, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 15, Status = AssignmentStatus.Draft,
            CreatedAt = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc)
        };
        var a3 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Chemical Bonding and Molecular Structure",
            Description = "Explain ionic, covalent and metallic bonding with suitable examples and draw the Lewis structures for the listed molecules.",
            SubjectId = chemistry.Id, ClassId = xiSciA.Id, TeacherId = rakib.Id,
            Deadline = new DateTime(2026, 8, 12, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 25, Status = AssignmentStatus.Closed,
            CreatedAt = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 7, 20, 8, 0, 0, DateTimeKind.Utc)
        };
        var a4 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Cell Division and Its Importance",
            Description = "Compare mitosis and meiosis and describe the biological importance of each with labelled diagrams.",
            SubjectId = biology.Id, ClassId = xiSciB.Id, TeacherId = abdul.Id,
            Deadline = new DateTime(2026, 8, 20, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 20, Status = AssignmentStatus.Published,
            CreatedAt = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 8, 3, 9, 0, 0, DateTimeKind.Utc)
        };
        var a5 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Preparation of Journal Entries — Assignment 01",
            Description = "Record the given transactions as journal entries following the double-entry system and prepare the ledger accounts.",
            SubjectId = accounting.Id, ClassId = xiBusA.Id, TeacherId = farhana.Id,
            Deadline = new DateTime(2026, 8, 22, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 20, Status = AssignmentStatus.Published,
            CreatedAt = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 8, 5, 10, 0, 0, DateTimeKind.Utc)
        };
        var a6 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Fundamental Rights and Duties of Citizens",
            Description = "Discuss the fundamental rights guaranteed by the Constitution of Bangladesh and explain the corresponding duties of citizens.",
            SubjectId = civics.Id, ClassId = xiHumA.Id, TeacherId = sharmin.Id,
            Deadline = new DateTime(2026, 8, 28, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 30, Status = AssignmentStatus.Published,
            CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 8, 8, 8, 0, 0, DateTimeKind.Utc)
        };
        var a7 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Number Systems and Digital Devices",
            Description = "Convert between binary, octal, decimal and hexadecimal number systems and explain the function of common digital devices.",
            SubjectId = ict.Id, ClassId = xiSciA.Id, TeacherId = nusrat.Id,
            Deadline = new DateTime(2026, 8, 5, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 20, Status = AssignmentStatus.Closed,
            CreatedAt = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 7, 15, 10, 0, 0, DateTimeKind.Utc)
        };
        var a8 = new Assignment
        {
            Id = Guid.NewGuid(), Title = "Periodic Properties of Elements",
            Description = "Explain the trends in atomic radius, ionisation energy and electronegativity across a period and down a group.",
            SubjectId = chemistry.Id, ClassId = xiSciA.Id, TeacherId = rakib.Id,
            Deadline = new DateTime(2026, 8, 30, 23, 59, 0, DateTimeKind.Utc),
            MaximumMarks = 15, Status = AssignmentStatus.Published,
            CreatedAt = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            PublishedAt = new DateTime(2026, 8, 10, 9, 0, 0, DateTimeKind.Utc)
        };

        _context.Assignments.AddRange(a1, a2, a3, a4, a5, a6, a7, a8);

        // ─── Submissions ───────────────────────────────────────
        var s1 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a1.Id, StudentId = fahim.Id,
            TextAnswer = "Newton's First Law states that an object at rest stays at rest and an object in motion stays in motion unless acted upon by an external force...",
            Status = SubmissionStatus.Submitted,
            SubmittedAt = new DateTime(2026, 8, 12, 10, 32, 0, DateTimeKind.Utc)
        };
        var s2 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a1.Id, StudentId = sadia.Id,
            TextAnswer = "Newton's three laws of motion form the foundation of classical mechanics...",
            Status = SubmissionStatus.Graded, Marks = 18,
            TeacherFeedback = "Excellent explanation of the second law. Recheck the unit conversion in problem 3.",
            SubmittedAt = new DateTime(2026, 8, 12, 9, 14, 0, DateTimeKind.Utc),
            GradedAt = new DateTime(2026, 8, 13, 15, 0, 0, DateTimeKind.Utc)
        };
        var s3 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a1.Id, StudentId = arif.Id,
            TextAnswer = "The laws of motion describe the relationship between force and motion...",
            Status = SubmissionStatus.Late,
            SubmittedAt = new DateTime(2026, 8, 19, 1, 22, 0, DateTimeKind.Utc) // After deadline
        };
        var s4 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a4.Id, StudentId = mehedi.Id,
            TextAnswer = "Mitosis produces two identical daughter cells while meiosis produces four genetically different cells...",
            Status = SubmissionStatus.Graded, Marks = 16,
            TeacherFeedback = "Good diagrams. Add more detail on the phases of meiosis.",
            SubmittedAt = new DateTime(2026, 8, 11, 20, 40, 0, DateTimeKind.Utc),
            GradedAt = new DateTime(2026, 8, 14, 10, 0, 0, DateTimeKind.Utc)
        };
        var s5 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a6.Id, StudentId = sumaiya.Id,
            TextAnswer = "The Constitution of Bangladesh guarantees several fundamental rights to its citizens...",
            Status = SubmissionStatus.Returned, Marks = 22,
            TeacherFeedback = "Please expand the section on citizens' duties with concrete examples.",
            SubmittedAt = new DateTime(2026, 8, 14, 15, 11, 0, DateTimeKind.Utc),
            GradedAt = new DateTime(2026, 8, 15, 9, 0, 0, DateTimeKind.Utc)
        };
        var s6 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a3.Id, StudentId = fahim.Id,
            TextAnswer = "Chemical bonding involves the sharing or transfer of electrons between atoms...",
            Status = SubmissionStatus.Graded, Marks = 22,
            TeacherFeedback = "Well-structured answer. Lewis structures are correctly drawn.",
            SubmittedAt = new DateTime(2026, 8, 10, 14, 0, 0, DateTimeKind.Utc),
            GradedAt = new DateTime(2026, 8, 13, 11, 0, 0, DateTimeKind.Utc)
        };
        var s7 = new Submission
        {
            Id = Guid.NewGuid(), AssignmentId = a7.Id, StudentId = fahim.Id,
            TextAnswer = "Binary number system uses base-2 with digits 0 and 1...",
            Status = SubmissionStatus.Graded, Marks = 17,
            TeacherFeedback = "Good work on conversions. The digital device explanations could be more detailed.",
            SubmittedAt = new DateTime(2026, 8, 4, 16, 0, 0, DateTimeKind.Utc),
            GradedAt = new DateTime(2026, 8, 6, 10, 0, 0, DateTimeKind.Utc)
        };

        _context.Submissions.AddRange(s1, s2, s3, s4, s5, s6, s7);

        // ─── Notifications ───────────────────────────────────────
        var notificationList = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), UserId = fahimUser.Id, Title = "New Assignment", Message = "Nusrat Jahan published a new assignment: Limits and Continuity — Problem Set.", Type = "assignment_published", CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
            new() { Id = Guid.NewGuid(), UserId = fahimUser.Id, Title = "Deadline Reminder", Message = "Your Newton's Laws of Motion assignment is due tomorrow.", Type = "deadline_reminder", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = Guid.NewGuid(), UserId = fahimUser.Id, Title = "Assignment Graded", Message = "Your ICT assignment has been graded — 17/20.", Type = "graded", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.NewGuid(), UserId = fahimUser.Id, Title = "Feedback Received", Message = "You received feedback from Md. Rakib Hasan.", Type = "feedback", IsRead = true, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new() { Id = Guid.NewGuid(), UserId = adminUser.Id, Title = "Teacher Assigned", Message = "Sharmin Sultana assigned to XI Humanities A — Civics and Good Governance.", Type = "teacher_assigned", IsRead = true, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new() { Id = Guid.NewGuid(), UserId = abdulUser.Id, Title = "Submission Received", Message = "Fahim Rahman submitted Newton's Laws of Motion — Assignment 01.", Type = "submission_received", CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new() { Id = Guid.NewGuid(), UserId = abdulUser.Id, Title = "Late Submission", Message = "Arif Hossain submitted Newton's Laws of Motion — Assignment 01 after the deadline.", Type = "late_submission", CreatedAt = DateTime.UtcNow.AddDays(-1) },
        };
        _context.Notifications.AddRange(notificationList);

        // ─── Application Settings ───────────────────────────────────────
        _context.ApplicationSettings.AddRange(
            new ApplicationSetting { Id = Guid.NewGuid(), Key = "AllowResubmission", Value = "true", Description = "Allow students to resubmit assignments before the deadline" },
            new ApplicationSetting { Id = Guid.NewGuid(), Key = "AllowLateSubmission", Value = "false", Description = "Allow students to submit assignments after the deadline" },
            new ApplicationSetting { Id = Guid.NewGuid(), Key = "MaxFileSize", Value = "10485760", Description = "Maximum file upload size in bytes (default 10MB)" },
            new ApplicationSetting { Id = Guid.NewGuid(), Key = "AllowedFileTypes", Value = ".pdf,.doc,.docx,.jpg,.jpeg,.png,.txt,.xlsx,.pptx", Description = "Allowed file extensions for uploads" }
        );

        await _context.SaveChangesAsync();
        _logger.LogInformation("Database seeded successfully with {UserCount} users, {ClassCount} classes, {AssignmentCount} assignments, {SubmissionCount} submissions.",
            17, 7, 8, 7);
    }
}
