using Domain.Enums;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.DTOs;

public class UpdateTaskStatusDto
{
    public TaskStatus Status { get; set; }
}
