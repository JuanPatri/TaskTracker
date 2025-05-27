using Task = Backend.Domain.Task;

namespace Backend.DTOs.TaskDTOs;

public class GetTaskDTO
{
    public string Title { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Status { get; set; } = "";
    public DateTime EarlyStart { get; set; }
    public DateTime EarlyFinish { get; set; }
    public DateTime LateStart { get; set; }
    public DateTime LateFinish { get; set; }
    public int Slack => (LateStart - EarlyStart).Days;
    public DateTime? DateCompleated { get; set; }
}