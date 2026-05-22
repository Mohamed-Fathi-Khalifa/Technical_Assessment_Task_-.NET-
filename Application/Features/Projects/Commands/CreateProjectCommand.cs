using Application.Common;
using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Commands;

public record CreateProjectCommand(string Name, string? Description)
    : IRequest<ApiResponse<ProjectResponseDto>>;
