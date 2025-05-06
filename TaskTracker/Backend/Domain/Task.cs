namespace Backend.Domain;

using Enums;
using DTOs.TaskDTOs;
using  Backend.Service;

public class Task
{
    private string _title = String.Empty;
    private string _description = String.Empty;
    private TimeSpan _duration;
    private Status _status = Status.Pending;
    private List<Task> _dependencies = new List<Task>();
    private List<(int, Resource)> _resources = new List<(int, Resource)>();

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The title name cannot be empty");
            _title = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The description cannot be empty");
            }

            _description = value;
        }
    }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            if (value < TimeSpan.FromMinutes(30))
                throw new ArgumentException("The duration must be at least 30 minutes");
            _duration = value;
        }
    }

    public Status Status
    {
        get => _status;
        set => _status = value;
    }
    
    public List<Task> Dependencies
    {
        get => _dependencies;
        set => _dependencies = value;
    }
    
    public List<(int, Resource)> Resources
    {
        get => _resources;
        set
        {
            _resources = value;
        } 
    }
    
    public static Task FromDto(TaskDataDTO taskDataDto, List<Task> dependencies, List<(int, Resource)> resource)
    {
        return new Task()
        {
            Title = taskDataDto.Title,
            Description = taskDataDto.Description,
            Duration = taskDataDto.Duration,
            Status = taskDataDto.Status,
            Dependencies = dependencies,
            Resources = resource
        };
    }
    
}