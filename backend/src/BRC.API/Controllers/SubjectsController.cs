using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Subjects;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize]
public class SubjectsController : ApiControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SubjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjects([FromQuery] PaginationParams pagination, [FromQuery] string? search, [FromQuery] Guid? groupId)
    {
        var result = await _subjectService.GetSubjectsAsync(pagination, search, groupId);
        return OkResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubject(Guid id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        if (subject == null) return ErrorResult("Subject not found", StatusCodes.Status404NotFound);
        return OkResult(subject);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
    {
        var subject = await _subjectService.CreateSubjectAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SubjectDto>.Ok(subject, "Subject created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubjectDto dto)
    {
        var subject = await _subjectService.UpdateSubjectAsync(id, dto);
        if (subject == null) return ErrorResult("Subject not found", StatusCodes.Status404NotFound);
        return OkResult(subject, "Subject updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        var success = await _subjectService.DeleteSubjectAsync(id);
        if (!success) return ErrorResult("Subject not found", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("Subject deleted successfully"));
    }
}
