using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projects.Queries;

public class GetProjectByIdQueryHandler
    : IRequestHandler<GetProjectByIdQuery, ApiResponse<ProjectResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProjectByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectResponseDto>> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            throw new KeyNotFoundException($"Project with ID {request.Id} was not found.");

        if (project.UserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("You do not have permission to view this project.");

        return ApiResponse<ProjectResponseDto>.Ok(new ProjectResponseDto
        {
            Id          = project.Id,
            Name        = project.Name,
            Description = project.Description,
            CreatedAt   = project.CreatedAt,
            UserId      = project.UserId
        });
    }
}
