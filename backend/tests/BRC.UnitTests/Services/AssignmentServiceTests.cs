using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BRC.Application.DTOs.Assignments;
using BRC.Application.Services;
using BRC.Domain.Entities;
using BRC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BRC.UnitTests.Services
{
    public class AssignmentServiceTests
    {
        private BrcDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BrcDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new BrcDbContext(options);
        }

        [Fact]
        public async Task GetAssignmentById_ShouldReturnAssignment_WhenExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var loggerMock = new Mock<ILogger<AssignmentService>>();
            var assignmentId = Guid.NewGuid();
            
            var subjectId = Guid.NewGuid();
            var classId = Guid.NewGuid();
            var teacherId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            context.Subjects.Add(new Subject { Id = subjectId, Code = "CS101", Name = "Computer Science" });
            context.Classes.Add(new Class { Id = classId, Name = "Class 10", AcademicYear = "2024", AcademicGroupId = Guid.NewGuid() });
            context.Users.Add(new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Teachers.Add(new Teacher { Id = teacherId, EmployeeId = "T1001", UserId = userId });

            var assignment = new Assignment
            {
                Id = assignmentId,
                Title = "Test Assignment",
                Description = "Test Description",
                MaximumMarks = 100,
                Deadline = DateTime.UtcNow.AddDays(7),
                TeacherId = teacherId,
                SubjectId = subjectId,
                ClassId = classId,
                Status = BRC.Domain.Enums.AssignmentStatus.Published
            };
            
            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();

            var service = new AssignmentService(context, loggerMock.Object);

            // Act
            var result = await service.GetAssignmentByIdAsync(assignmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Assignment", result.Title);
            Assert.Equal(100, result.MaximumMarks);
        }
    }
}
