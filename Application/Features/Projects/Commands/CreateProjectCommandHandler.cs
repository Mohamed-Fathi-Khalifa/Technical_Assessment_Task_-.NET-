using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Projects.Commands;

public class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, ApiResponse<ProjectResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateProjectCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectResponseDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name        = request.Name,
            Description = request.Description ?? string.Empty,
            CreatedAt   = DateTime.UtcNow,
            UserId      = _currentUser.UserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProjectResponseDto>.Ok(MapToDto(project), "Project created successfully.");
    }

    private static ProjectResponseDto MapToDto(Project p) => new()
    {
        Id          = p.Id,
        Name        = p.Name,
        Description = p.Description,
        CreatedAt   = p.CreatedAt,
        UserId      = p.UserId
    };
}
