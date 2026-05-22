using Application.Common;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_.NET_Developer___Technical_Assessment_Task.Controllers;

[Authorize]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get all projects for the authenticated user.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProjectResponseDto>>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllProjectsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Get a single project by ID (must belong to the current user).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id), ct);
        return Ok(result);
    }

    /// <summary>Create a new project.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create(
        [FromBody] CreateProjectDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProjectCommand(dto.Name, dto.Description), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Update an existing project (must belong to the current user).</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(
        int id,
        [FromBody] UpdateProjectDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateProjectCommand(id, dto.Name, dto.Description), ct);
        return Ok(result);
    }

    /// <summary>Delete a project and all its tasks (must belong to the current user).</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteProjectCommand(id), ct);
        return Ok(result);
    }
}
