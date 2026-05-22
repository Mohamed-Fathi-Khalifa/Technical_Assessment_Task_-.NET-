using Application.Common;
using Application.Features.Tasks.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, ApiResponse<TaskResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<TaskResponseDto>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        // Security: verify the project belongs to the current user
        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Project with ID {request.ProjectId} was not found.");

        if (project.UserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("You do not have permission to add tasks to this project.");

        var task = new TaskItem
        {
            Title       = request.Title,
            Description = request.Description ?? string.Empty,
            DueDate     = request.DueDate,
            Priority    = request.Priority,
            Status      = TaskStatus.Todo,
            ProjectId   = request.ProjectId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<TaskResponseDto>.Ok(MapToDto(task), "Task created successfully.");
    }

    private static TaskResponseDto MapToDto(TaskItem t) => new()
    {
        Id          = t.Id,
        Title       = t.Title,
        Description = t.Description,
        Status      = t.Status,
        DueDate     = t.DueDate,
        Priority    = t.Priority,
        ProjectId   = t.ProjectId
    };
}
