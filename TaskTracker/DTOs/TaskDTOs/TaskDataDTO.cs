using Enums;

namespace DTOs.TaskDTOs;

public class TaskDataDTO
{ 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; } = 1;
    public Status Status { get; set; } = Status.Pending;
    public List<String> Dependencies { get; set; } = new List<string>();
    public List<(int, string)> Resources { get; set; } = new List<(int, string)>();
    public DateTime? DateCompleated { get; set; } = null;
}