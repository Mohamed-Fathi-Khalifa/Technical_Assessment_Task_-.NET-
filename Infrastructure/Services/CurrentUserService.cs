using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // Try "sub" first (default in .NET 9 — MapInboundClaims = false)
            var raw = user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(raw, out var id) ? id : 0;
        }
    }
}
