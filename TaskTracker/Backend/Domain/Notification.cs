using Backend.Domain.Enums;

namespace Backend.Domain;

public class Notification
{
    private string _message;
    private DateTime _date;
    private TypeOfNotification _typeOfNotification;
    private int _impact;
    private List<Task> _task;
    private List<User> _users;
    private List<Project> _projects;
    
    private const int MinImpact = 1;
    public string Message
    {
        get => _message;
        set 
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("The notification message cannot be empty");
            _message = value;
        } 
    }
    
    public DateTime Date
    {
        get => _date;
        set 
        {
            if (value < DateTime.Now) throw new ArgumentException("The notification date cannot be in the past");
            _date = value;
        }
    }
    
    public TypeOfNotification TypeOfNotification
    {
        get => _typeOfNotification;
        set => _typeOfNotification = value;
    }
    
    public int Impact
    {
        get => _impact;
        set
        {
            if (value < MinImpact) throw new ArgumentException("The impact must be greater than 0");
            _impact = value;
        }
    }
    
    public List<Task> Task
    {
        get => _task;
        set
        {
            if (value == null) throw new ArgumentNullException("The task cannot be null");
            _task = value;
        }
    }
    public List<User> Users
    {
        get => _users;
        set
        {
            if (value == null) throw new ArgumentNullException("The users cannot be null");
            _users = value;
        }
    }
    public List<Project> Projects
    {
        get => _projects;
        set
        {
            if (value == null) throw new ArgumentNullException("The projects cannot be null");
            _projects = value;
        }
    }
}