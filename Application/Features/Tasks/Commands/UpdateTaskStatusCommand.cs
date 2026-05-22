using Application.Common;
using Application.Features.Tasks.DTOs;
using Domain.Enums;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands;

public record UpdateTaskStatusCommand(int TaskId, TaskStatus Status)
    : IRequest<ApiResponse<TaskResponseDto>>;
