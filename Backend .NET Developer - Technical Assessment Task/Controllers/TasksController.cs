using Application.Common;
using Application.Features.Tasks.Commands;
using Application.Features.Tasks.DTOs;
using Application.Features.Tasks.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_.NET_Developer___Technical_Assessment_Task.Controllers;

[Authorize]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get all tasks for a project (project must belong to the current user).</summary>
    [HttpGet("projects/{projectId:int}/tasks")]
    public async Task<ActionResult<ApiResponse<List<TaskResponseDto>>>> GetByProject(
        int projectId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery(projectId), ct);
        return Ok(result);
    }

    /// <summary>Create a new task inside a project.</summary>
    [HttpPost("projects/{projectId:int}/tasks")]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> Create(
        int projectId,
        [FromBody] CreateTaskDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateTaskCommand(dto.Title, dto.Description, dto.DueDate, dto.Priority, projectId), ct);
        return Ok(result);
    }

    /// <summary>Update the status of a task.</summary>
    [HttpPatch("tasks/{id:int}/status")]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> UpdateStatus(
        int id,
        [FromBody] UpdateTaskStatusDto dto,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(id, dto.Status), ct);
        return Ok(result);
    }

    /// <summary>Delete a task (project must belong to the current user).</summary>
    [HttpDelete("tasks/{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(id), ct);
        return Ok(result);
    }
}
