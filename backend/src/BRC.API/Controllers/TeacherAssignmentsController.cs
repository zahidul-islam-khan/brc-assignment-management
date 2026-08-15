using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.TeacherAssignments;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/teacher-assignments")]
public class TeacherAssignmentsController : ApiControllerBase
{
    private readonly ITeacherAssignmentService _service;

    public TeacherAssignmentsController(ITeacherAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TeacherAssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] PaginationParams pagination, [FromQuery] Guid? teacherId, [FromQuery] Guid? classId, [FromQuery] Guid? subjectId)
    {
        var result = await _service.GetTeacherAssignmentsAsync(pagination, teacherId, classId, subjectId);
        return OkResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TeacherAssignmentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTeacherAssignmentDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TeacherAssignmentDto>.Ok(result, "Teacher assigned successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return ErrorResult("Assignment not found", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("Assignment removed successfully"));
    }
}
