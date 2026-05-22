using Application.Common;
using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries;

public record GetAllProjectsQuery : IRequest<ApiResponse<List<ProjectResponseDto>>>;
