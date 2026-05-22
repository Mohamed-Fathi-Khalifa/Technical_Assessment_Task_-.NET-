using Application.Common;
using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries;

public record GetAllProjectsAdminQuery() : IRequest<ApiResponse<List<ProjectResponseDto>>>;
