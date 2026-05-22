using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projects.Queries;

public class GetAllProjectsAdminQueryHandler : IRequestHandler<GetAllProjectsAdminQuery, ApiResponse<List<ProjectResponseDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProjectsAdminQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<ProjectResponseDto>>> Handle(GetAllProjectsAdminQuery request, CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<ProjectResponseDto>>.Ok(projects, "All projects retrieved successfully (Admin).");
    }
}
