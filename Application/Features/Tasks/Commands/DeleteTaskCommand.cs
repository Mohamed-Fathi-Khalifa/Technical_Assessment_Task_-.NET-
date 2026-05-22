using Application.Common;
using MediatR;

namespace Application.Features.Tasks.Commands;

public record DeleteTaskCommand(int TaskId) : IRequest<ApiResponse<bool>>;
