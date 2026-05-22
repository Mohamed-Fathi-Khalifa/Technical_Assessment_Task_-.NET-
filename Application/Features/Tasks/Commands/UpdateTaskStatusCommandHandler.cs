using Application.Common;
using Application.Features.Tasks.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands;

public class UpdateTaskStatusCommandHandler
    : IRequestHandler<UpdateTaskStatusCommand, ApiResponse<TaskResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<TaskResponseDto>> Handle(
        UpdateTaskStatusCommand request,
        CancellationToken cancellationToken)
    {
        // Load task with its parent project to verify ownership
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Task with ID {request.TaskId} was not found.");

        if (task.Project!.UserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("You do not have permission to update this task.");

        task.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<TaskResponseDto>.Ok(new TaskResponseDto
        {
            Id          = task.Id,
            Title       = task.Title,
            Description = task.Description,
            Status      = task.Status,
            DueDate     = task.DueDate,
            Priority    = task.Priority,
            ProjectId   = task.ProjectId
        }, "Task status updated successfully.");
    }
}
