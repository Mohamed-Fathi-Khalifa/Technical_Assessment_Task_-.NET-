using Application.Common;
using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries;

public record GetTasksByProjectQuery(int ProjectId)
    : IRequest<ApiResponse<List<TaskResponseDto>>>;
