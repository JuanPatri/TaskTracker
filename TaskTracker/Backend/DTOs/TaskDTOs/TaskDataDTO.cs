using Backend.Domain.Enums;

namespace Backend.DTOs.TaskDTOs;

public class TaskDataDTO
{ 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; } = 1;
    public Status Status { get; set; } = Status.Pending;
    public List<String> Dependencies { get; set; } = new List<string>();
    public List<(int, string)> Resources { get; set; } = new List<(int, string)>();
    public List<string> FinishToStartDependencies { get; set; } = new List<string>();
    public List<string> StartToStartDependencies { get; set; } = new List<string>();
    public double Slack { get; set; } = 0;
}