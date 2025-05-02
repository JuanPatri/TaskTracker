namespace Backend.Domain;
using Enums;
public class ProjectTask
{
    private string _title = String.Empty; 
    private string _description = String.Empty;
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    private TimeSpan _durationTask;
    private Status _status = Status.Pending;
    private Project _project;
    private List<ProjectTask> _dependencies = new List<ProjectTask>();

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
        set => _description = value;
    }
    public DateOnly Date
    {
        get => _date;
        set
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            if (value > today)
                throw new ArgumentException("The date cannot be in the future.");
            
            _date = value;
        }
    }

    public TimeSpan DurationTask
    {
        get => _durationTask;
        set => _durationTask = value;
    }

    public Status Status
    {
        get => _status;
        set => _status = value;
    }
    public Project Project
    {
        get => _project;
        set
        {
            
            if(value == null)
                throw new ArgumentException("Project cannot be null");
            
            _project = value;
        } 
    }

    public List<ProjectTask> Dependencies
    {
        get => _dependencies;
        set => _dependencies = value;
    }
    
    }