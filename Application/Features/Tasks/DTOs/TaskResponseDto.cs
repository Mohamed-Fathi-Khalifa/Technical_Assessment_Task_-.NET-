using Domain.Enums;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.DTOs;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
}
