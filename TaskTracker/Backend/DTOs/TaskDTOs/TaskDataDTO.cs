using Backend.Domain.Enums;
using Backend.Domain;

namespace Backend.DTOs.TaskDTOs;

public class TaskDataDTO
{ 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public Status Status { get; set; } = Status.Pending;
    public Project Project { get; set; } = new Project();
    public List<string> Dependencies { get; set; } = new List<string>();
}