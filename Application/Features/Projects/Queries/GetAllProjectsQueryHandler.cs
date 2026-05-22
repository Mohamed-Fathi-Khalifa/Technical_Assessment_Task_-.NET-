using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Features.Projects.Queries;

public class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, ApiResponse<List<ProjectResponseDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDistributedCache _cache;

    private const int CacheExpiryMinutes = 5;

    public GetAllProjectsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDistributedCache cache)
    {
        _context     = context;
        _currentUser = currentUser;
        _cache       = cache;
    }

    public async Task<ApiResponse<List<ProjectResponseDto>>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"projects-user-{_currentUser.UserId}";

        // 1. Try cache first
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            var cachedList = JsonSerializer.Deserialize<List<ProjectResponseDto>>(cached);
            return ApiResponse<List<ProjectResponseDto>>.Ok(cachedList!);
        }

        // 2. Cache miss — fetch from database
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

        // 3. Store in cache with expiry
        var serialized = JsonSerializer.Serialize(projects);
        await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpiryMinutes)
        }, cancellationToken);

        return ApiResponse<List<ProjectResponseDto>>.Ok(projects);
    }
}
