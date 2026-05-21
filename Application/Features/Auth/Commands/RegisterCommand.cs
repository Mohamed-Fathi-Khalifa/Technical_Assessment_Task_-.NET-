using Application.Common;
using MediatR;

namespace Application.Features.Auth.Commands;

public record RegisterCommand(
    string Name,
    string Email,
    string Password
) : IRequest<ApiResponse<AuthResult>>;
