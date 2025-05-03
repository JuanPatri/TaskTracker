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
    public List<string> Dependencies { get; set; } = new List<string>();
    
    public Task ToEntity()
    {

        return new Task()
        {
            Title = Title,
            Description = Description,
            Duration = Duration,
            Status = Status,
            // Project = _projects.Find(p => p.id  == Project), //Hacer en service project
            // Dependencies = taskDependencies
        };
    }
}