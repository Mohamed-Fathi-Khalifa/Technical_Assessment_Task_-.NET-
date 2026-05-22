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

        // ── 1. Try the cache (read) ───────────────────────────────────────────
        // If Redis is unavailable (timeout, connection refused, etc.) we catch
        // the exception silently and fall through to the database.
        try
        {
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (cached is not null)
            {
                var cachedList = JsonSerializer.Deserialize<List<ProjectResponseDto>>(cached);
                return ApiResponse<List<ProjectResponseDto>>.Ok(cachedList!);
            }
        }
        catch
        {
            // Redis unavailable — proceed to database fallback
        }

        // ── 2. Cache miss (or Redis down) — fetch from the database ──────────
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

        // ── 3. Try to populate the cache (write) ─────────────────────────────
        // A caching failure must NEVER prevent the caller from receiving data.
        try
        {
            var serialized = JsonSerializer.Serialize(projects);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpiryMinutes)
            }, cancellationToken);
        }
        catch
        {
            // Redis unavailable — DB data is returned successfully regardless
        }

        return ApiResponse<List<ProjectResponseDto>>.Ok(projects);
    }
}
