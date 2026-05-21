using Application.Common;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly LoginCommandValidator _validator;

    public LoginCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        LoginCommandValidator validator)
    {
        _context = context;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<ApiResponse<AuthResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<AuthResult>.Fail("Validation failed.", errors);
        }

        // 2. Lookup user — generic message prevents user enumeration
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResult>.Fail("Login failed.", "Invalid email or password.");

        // 3. Generate token and return
        var token  = _tokenService.GenerateToken(user);
        var result = new AuthResult(user.Id, user.Email, user.Name, token);

        return ApiResponse<AuthResult>.Ok(result, "Login successful.");
    }
}
