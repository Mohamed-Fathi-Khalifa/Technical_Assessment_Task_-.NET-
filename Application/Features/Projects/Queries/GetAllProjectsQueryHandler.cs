using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projects.Queries;

public class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, ApiResponse<List<ProjectResponseDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllProjectsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<List<ProjectResponseDto>>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.UserId == _currentUser.UserId)
            .Select(p => new ProjectResponseDto
            {
                Id          = p.Id,
                Name        = p.Name,
                Description = p.Description,
                CreatedAt   = p.CreatedAt,
                UserId      = p.UserId
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<ProjectResponseDto>>.Ok(projects);
    }
}
