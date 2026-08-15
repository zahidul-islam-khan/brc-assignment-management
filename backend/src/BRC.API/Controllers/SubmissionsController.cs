using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Submissions;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize]
public class SubmissionsController : ApiControllerBase
{
    private readonly ISubmissionService _submissionService;
    private readonly BRC.Infrastructure.Data.BrcDbContext _context;

    public SubmissionsController(ISubmissionService submissionService, BRC.Infrastructure.Data.BrcDbContext context)
    {
        _submissionService = submissionService;
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Teacher,Student")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SubmissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubmissions([FromQuery] PaginationParams pagination, [FromQuery] SubmissionFilterParams filters)
    {
        if (CurrentUserRole == "Admin")
        {
            var result = await _submissionService.GetSubmissionsAsync(pagination, filters);
            return OkResult(result);
        }
        else if (CurrentUserRole == "Student")
        {
            var studentId = await GetStudentIdAsync(_context);
            if (studentId == null) return ErrorResult("Student profile not found");
            
            var result = await _submissionService.GetStudentSubmissionsAsync(studentId.Value, pagination, filters);
            return OkResult(result);
        }
        else // Teacher
        {
            var teacherId = await GetTeacherIdAsync(_context);
            if (teacherId == null) return ErrorResult("Teacher profile not found");
            
            var result = await _submissionService.GetSubmissionsAsync(pagination, filters);
            return OkResult(result);
        }
    }

    [HttpGet("assignment/{assignmentId}")]
    [Authorize(Roles = "Teacher")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SubmissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignmentSubmissions(Guid assignmentId, [FromQuery] PaginationParams pagination)
    {
        var teacherId = await GetTeacherIdAsync(_context);
        if (teacherId == null) return ErrorResult("Teacher profile not found");

        var result = await _submissionService.GetAssignmentSubmissionsAsync(assignmentId, teacherId.Value, pagination);
        return OkResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubmission(Guid id)
    {
        if (CurrentUserRole == "Student")
        {
            var studentId = await GetStudentIdAsync(_context);
            if (studentId == null) return ErrorResult("Student profile not found");

            var result = await _submissionService.GetStudentSubmissionByIdAsync(id, studentId.Value);
            if (result == null) return ErrorResult("Submission not found", StatusCodes.Status404NotFound);
            return OkResult(result);
        }
        else
        {
            var result = await _submissionService.GetSubmissionByIdAsync(id);
            if (result == null) return ErrorResult("Submission not found", StatusCodes.Status404NotFound);
            return OkResult(result);
        }
    }

    [HttpPost("assignment/{assignmentId}")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSubmission(Guid assignmentId, [FromBody] CreateSubmissionDto dto)
    {
        var studentId = await GetStudentIdAsync(_context);
        if (studentId == null) return ErrorResult("Student profile not found");

        var result = await _submissionService.CreateSubmissionAsync(assignmentId, studentId.Value, dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SubmissionDto>.Ok(result, "Submission created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionDto dto)
    {
        var studentId = await GetStudentIdAsync(_context);
        if (studentId == null) return ErrorResult("Student profile not found");

        var result = await _submissionService.UpdateSubmissionAsync(id, studentId.Value, dto);
        if (result == null) return ErrorResult("Submission not found or unauthorized", StatusCodes.Status404NotFound);
        return OkResult(result, "Submission updated successfully");
    }

    [HttpPatch("{id}/submit")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitSubmission(Guid id)
    {
        var studentId = await GetStudentIdAsync(_context);
        if (studentId == null) return ErrorResult("Student profile not found");

        var result = await _submissionService.SubmitSubmissionAsync(id, studentId.Value);
        if (result == null) return ErrorResult("Submission not found or unauthorized", StatusCodes.Status404NotFound);
        return OkResult(result, "Assignment submitted successfully");
    }

    [HttpPost("{id}/upload")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
    {
        var studentId = await GetStudentIdAsync(_context);
        if (studentId == null) return ErrorResult("Student profile not found");

        if (file == null || file.Length == 0)
            return ErrorResult("File is empty", StatusCodes.Status400BadRequest);

        var result = await _submissionService.UploadFileAsync(id, studentId.Value, file);
        if (result == null) return ErrorResult("Submission not found or unauthorized", StatusCodes.Status404NotFound);
        return OkResult(result, "File uploaded successfully");
    }

    [HttpPost("{id}/grade")]
    [Authorize(Roles = "Teacher")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GradeSubmission(Guid id, [FromBody] GradeSubmissionDto dto)
    {
        var teacherId = await GetTeacherIdAsync(_context);
        if (teacherId == null) return ErrorResult("Teacher profile not found");

        var result = await _submissionService.GradeSubmissionAsync(id, teacherId.Value, dto);
        if (result == null) return ErrorResult("Submission not found or unauthorized", StatusCodes.Status404NotFound);
        return OkResult(result, "Submission graded successfully");
    }
}
