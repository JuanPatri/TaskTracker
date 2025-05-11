namespace Backend.Domain;

using Enums;
using DTOs.TaskDTOs;
using  Backend.Service;

public class Task
{
    private string _title = String.Empty;
    private string _description = String.Empty;
    private double _duration;
    private Status _status = Status.Pending;
    private List<(int, Resource)> _resources = new List<(int, Resource)>();
    private List<Task> _dependencies = new List<Task>();
    private double _slack;
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

    public double Duration
    {
        get => _duration;
        set
        {
            if (value < 0.5)
                throw new ArgumentException("The duration must be at least 0.5 hours (30 minutes)");
            _duration = value;
        }
    }

    public Status Status
    {
        get => _status;
        set => _status = value;
    }
    
    public List<(int, Resource)> Resources
    {
        get => _resources;
        set
        {
            _resources = value;
        } 
    }
    
    public List<Task> Dependencies
    {
        get => _dependencies;
        set => _dependencies = value; 
    }
    
    public double Slack
    {
        get => _slack;
        set => _slack = value; 
    }
    public static Task FromDto(TaskDataDTO taskDataDto, List<(int, Resource)> resource, List<Task> dependencies)
    {
        return new Task()
        {
            Title = taskDataDto.Title,
            Description = taskDataDto.Description,
            Duration = taskDataDto.Duration,
            Status = taskDataDto.Status,
            Resources = resource,
            Dependencies = dependencies,
            Slack = taskDataDto.Slack
        };
    }
    
}