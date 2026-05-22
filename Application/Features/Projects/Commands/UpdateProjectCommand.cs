using Application.Common;
using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Commands;

public record UpdateProjectCommand(int Id, string Name, string? Description)
    : IRequest<ApiResponse<ProjectResponseDto>>;
