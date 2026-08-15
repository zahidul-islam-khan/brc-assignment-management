using BRC.Application.DTOs.Assignments;
using BRC.Application.DTOs.Common;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize]
public class AssignmentsController : ApiControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly BRC.Infrastructure.Data.BrcDbContext _context;

    public AssignmentsController(IAssignmentService assignmentService, BRC.Infrastructure.Data.BrcDbContext context)
    {
        _assignmentService = assignmentService;
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignments([FromQuery] PaginationParams pagination, [FromQuery] AssignmentFilterParams filters)
    {
        if (CurrentUserRole == "Admin")
        {
            var result = await _assignmentService.GetAllAssignmentsAsync(pagination, filters);
            return OkResult(result);
        }
        else if (CurrentUserRole == "Teacher")
        {
            var teacherId = await GetTeacherIdAsync(_context);
            if (teacherId == null) return ErrorResult("Teacher profile not found");
            
            var result = await _assignmentService.GetTeacherAssignmentsAsync(teacherId.Value, pagination, filters);
            return OkResult(result);
        }
        
        return ErrorResult("Unauthorized access.", StatusCodes.Status403Forbidden);
    }

    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StudentAssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentAssignments([FromQuery] PaginationParams pagination, [FromQuery] AssignmentFilterParams filters)
    {
        var studentId = await GetStudentIdAsync(_context);
        if (studentId == null) return ErrorResult("Student profile not found");

        var result = await _assignmentService.GetStudentAssignmentsAsync(studentId.Value, pagination, filters);
        return OkResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignment(Guid id)
    {
        if (CurrentUserRole == "Student")
        {
            var studentId = await GetStudentIdAsync(_context);
            if (studentId == null) return ErrorResult("Student profile not found");

            var studentResult = await _assignmentService.GetStudentAssignmentByIdAsync(id, studentId.Value);
            if (studentResult == null) return ErrorResult("Assignment not found", StatusCodes.Status404NotFound);
            return OkResult(studentResult);
        }

        var result = await _assignmentService.GetAssignmentByIdAsync(id);
        if (result == null) return ErrorResult("Assignment not found", StatusCodes.Status404NotFound);
        return OkResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto)
    {
        var teacherId = await GetTeacherIdAsync(_context);
        if (teacherId == null) return ErrorResult("Teacher profile not found");

        var result = await _assignmentService.CreateAssignmentAsync(teacherId.Value, dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AssignmentDto>.Ok(result, "Assignment created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] UpdateAssignmentDto dto)
    {
        var teacherId = await GetTeacherIdAsync(_context);
        if (teacherId == null) return ErrorResult("Teacher profile not found");

        var result = await _assignmentService.UpdateAssignmentAsync(id, teacherId.Value, dto);
        if (result == null) return ErrorResult("Assignment not found or unauthorized", StatusCodes.Status404NotFound);
        return OkResult(result, "Assignment updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var teacherId = await GetTeacherIdAsync(_context);
        if (teacherId == null) return ErrorResult("Teacher profile not found");

        var success = await _assignmentService.DeleteAssignmentAsync(id, teacherId.Value);
        if (!success) return ErrorResult("Assignment not found or unauthorized", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("Assignment deleted successfully"));
    }

    [HttpPatch("{id}/publish")]
    [Authorize(Roles = "Teacher")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishAssignment(Guid id)
    {
        var teacherId = await GetTeacherIdAsync(_context);
        if (teacherId == null) return ErrorResult("Teacher profile not found");

        var result = await _assignmentService.PublishAssignmentAsync(id, teacherId.Value);
        if (result == null) return ErrorResult("Assignment not found or unauthorized", StatusCodes.Status404NotFound);
        return OkResult(result, "Assignment published successfully");
    }
}
