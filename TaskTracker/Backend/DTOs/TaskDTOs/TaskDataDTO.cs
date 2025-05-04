using Backend.Domain;
using Backend.Domain.Enums;
using Task = Backend.Domain.Task;
using Backend.Repository;

namespace Backend.DTOs.TaskDTOs;

public class TaskDataDTO
{ 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public Status Status { get; set; } = Status.Pending;
    public int Project { get; set; } = 0;
    public List<String> Dependencies { get; set; } = new List<string>();
    
    public Task ToEntity(List<Task> dependencies, Project project)
    {

        return new Task()
        {
            Title = Title,
            Description = Description,
            Duration = Duration,
            Status = Status,
            Project = project,
            Dependencies = dependencies
        };
    }
}