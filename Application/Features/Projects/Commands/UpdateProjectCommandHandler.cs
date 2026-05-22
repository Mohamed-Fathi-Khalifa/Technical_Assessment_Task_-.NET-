using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projects.Commands;

public class UpdateProjectCommandHandler
    : IRequestHandler<UpdateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateProjectCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectResponseDto>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Project with ID {request.Id} was not found.");

        if (project.UserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("You do not have permission to update this project.");

        project.Name        = request.Name;
        project.Description = request.Description ?? string.Empty;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProjectResponseDto>.Ok(new ProjectResponseDto
        {
            Id          = project.Id,
            Name        = project.Name,
            Description = project.Description,
            CreatedAt   = project.CreatedAt,
            UserId      = project.UserId
        }, "Project updated successfully.");
    }
}
