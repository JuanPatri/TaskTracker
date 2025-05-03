using Task = Backend.Domain.Task;

namespace Backend.DTOs.TaskDTOs;

public class GetTaskDTO
{
    public string Title { get; set; } = string.Empty;
    
    public Task ToEntity()
    {
        return new Task()
        {
            Title = Title
        };
    }
}