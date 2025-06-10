using Enums;
using DTOs.TaskDTOs;
using Enums;

namespace Domain;

public class Task
{
    private string _title = String.Empty;
    private string _description = String.Empty;
    private int _duration;
    private Status _status = Status.Pending;
    private List<TaskResource> _resources = new List<TaskResource>();
    private List<Task> _dependencies = new List<Task>();
    public DateTime EarlyStart { get; set; }
    public DateTime EarlyFinish { get; set; }
    public DateTime? DateCompleated { get; set; }
    public DateTime LateStart { get; set; }
    public DateTime LateFinish { get; set; }
    
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

    public int Duration
    {
        get => _duration;
        set
        {
            if (value < 0.5)
                throw new ArgumentException("The duration must be at least 1 day");
            _duration = value;
        }
    }

    public Status Status
    {
        get => _status;
        set => _status = value;
    }
    
    public List<TaskResource> Resources
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
}