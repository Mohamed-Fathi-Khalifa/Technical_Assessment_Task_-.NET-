using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        RegisterCommandValidator validator)
    {
        _context = context;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<ApiResponse<AuthResult>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return ApiResponse<AuthResult>.Fail("Validation failed.", errors);
        }

        // 2. Check for duplicate email
        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailTaken)
            return ApiResponse<AuthResult>.Fail("Registration failed.", "Email is already in use.");

        // 3. Hash password and persist
        var user = new User
        {
            Name         = request.Name,
            Email        = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Generate token and return
        var token  = _tokenService.GenerateToken(user);
        var result = new AuthResult(user.Id, user.Email, user.Name, token);

        return ApiResponse<AuthResult>.Ok(result, "User registered successfully.");
    }
}
