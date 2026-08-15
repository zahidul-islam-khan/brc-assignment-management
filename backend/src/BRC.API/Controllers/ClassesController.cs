using BRC.Application.DTOs.Classes;
using BRC.Application.DTOs.Common;
using BRC.Application.DTOs.Subjects;
using BRC.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BRC.API.Controllers;

[Authorize]
public class ClassesController : ApiControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ClassDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClasses([FromQuery] PaginationParams pagination, [FromQuery] string? search, [FromQuery] Guid? groupId)
    {
        var result = await _classService.GetClassesAsync(pagination, search, groupId);
        return OkResult(result);
    }

    [HttpGet("academic-groups")]
    [ProducesResponseType(typeof(ApiResponse<List<AcademicGroupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAcademicGroups()
    {
        var result = await _classService.GetAcademicGroupsAsync();
        return OkResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ClassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClass(Guid id)
    {
        var cls = await _classService.GetClassByIdAsync(id);
        if (cls == null) return ErrorResult("Class not found", StatusCodes.Status404NotFound);
        return OkResult(cls);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ClassDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassDto dto)
    {
        var cls = await _classService.CreateClassAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ClassDto>.Ok(cls, "Class created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ClassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassDto dto)
    {
        var cls = await _classService.UpdateClassAsync(id, dto);
        if (cls == null) return ErrorResult("Class not found", StatusCodes.Status404NotFound);
        return OkResult(cls, "Class updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteClass(Guid id)
    {
        var success = await _classService.DeleteClassAsync(id);
        if (!success) return ErrorResult("Class not found", StatusCodes.Status404NotFound);
        return Ok(ApiResponse.Ok("Class deleted successfully"));
    }
}
