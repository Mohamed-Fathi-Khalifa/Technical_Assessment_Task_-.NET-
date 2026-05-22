using Application.Common;
using Application.Features.Tasks.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.Tasks.Commands;

public record CreateTaskCommand(
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskPriority Priority,
    int ProjectId
) : IRequest<ApiResponse<TaskResponseDto>>;
