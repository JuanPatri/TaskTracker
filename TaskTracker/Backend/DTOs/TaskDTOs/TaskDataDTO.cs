using Backend.Domain.Enums;
using Task = Backend.Domain.Task;
using Backend.Domain.Repository;

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
        List<Task> taskDependencies = new List<Task>();
        for(int i = 0; i < Dependencies.Count; i++)
        {
            taskDependencies.Add(_repositoryProject.Find(t => t.title == Dependencies[i])); 
        }
        return new Task()
        {
            Title = Title,
            Description = Description,
            Duration = Duration,
            Status = Status,
            Project = _repositoryProject.Find(p => p.id  == Project),
            Dependencies = taskDependencies
        };
    }
}