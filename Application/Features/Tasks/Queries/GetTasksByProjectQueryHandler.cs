using Application.Common;
using Application.Features.Tasks.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Queries;

public class GetTasksByProjectQueryHandler
    : IRequestHandler<GetTasksByProjectQuery, ApiResponse<List<TaskResponseDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTasksByProjectQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<List<TaskResponseDto>>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        // Security: verify the project belongs to the current user
        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Project with ID {request.ProjectId} was not found.");

        if (project.UserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("You do not have permission to view tasks for this project.");

        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == request.ProjectId)
            .Select(t => new TaskResponseDto
            {
                Id          = t.Id,
                Title       = t.Title,
                Description = t.Description,
                Status      = t.Status,
                DueDate     = t.DueDate,
                Priority    = t.Priority,
                ProjectId   = t.ProjectId
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<TaskResponseDto>>.Ok(tasks);
    }
}
