using DTOs.ProjectDTOs;

namespace Domain;

public class Project
{
    private int _id;
    private string _name;
    private string _description;
    private DateOnly _startDate;
    private User _administrator;
    
    private List<Task> _tasks = new List<Task>();
    private List<User> _users = new List<User>();
    private List<Resource> _exclusiveResources = new List<Resource>();
    private List<Task> _criticalPath = new List<Task>();

    private const int MaxDescriptionLength = 400;
    public string Name
    {
      get => _name;  
      set
      {
          if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The project name cannot be empty");
          _name = value;
      }
    }
    
    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The project description cannot be empty");
            if (value.Length > MaxDescriptionLength) throw new ArgumentException("The project description cannot exceed 400 characters");
            _description = value;
        }
    }
    public DateOnly StartDate 
    {
        get => _startDate;
        set 
        {
            if (value < DateOnly.FromDateTime(DateTime.Now)) throw new ArgumentException("The project start date cannot be in the past");
            _startDate = value;
        }
    }
    public User Administrator
    {
        get => _administrator;
        set
        {
            if (value == null) throw new ArgumentException("The project administrator cannot be null");
            _administrator = value;
        }
    }
    public int Id
    {
        get => _id;
        set
        {
            if (value <= 0) throw new ArgumentException("The project ID must be a positive integer");
            _id = value;
        }
    }

    public List<Task> Tasks
    {
        get => _tasks;
        set => _tasks = value;
        
    }
    
    public List<User> Users
    {
        get => _users;
        set => _users = value; 
    }
    
    public List<Resource> ExclusiveResources
    {
        get => _exclusiveResources;
        set => _exclusiveResources = value;
    }

    public List<Task> CriticalPath
    {
        get => _criticalPath;
        set => _criticalPath = value;
    }
    
    public static Project FromDto(ProjectDataDTO projectDataDto, List<User> users)
    {
        return new Project()
        {
            Id = projectDataDto.Id,
            Name = projectDataDto.Name,
            Description = projectDataDto.Description,
            StartDate = projectDataDto.StartDate,
            Administrator = User.FromDto(projectDataDto.Administrator),
            Users = users
        };
    }
    
    
}