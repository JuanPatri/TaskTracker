using DTOs.ProjectDTOs;

namespace Domain;

public class Project
{
    private int _id;
    private string _name;
    private string _description;
    private DateOnly _startDate;
    private List<Task> _tasks = new List<Task>();
    private List<Resource> _exclusiveResources = new List<Resource>();
    private List<Task> _criticalPath = new List<Task>();
    private List<ProjectRole> _projectRoles = new List<ProjectRole>();
    
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
            _startDate = value;
        }
    }
    public int Id
    {
        get => _id;
        set
        {
            _id = value;
        }
    }

    public List<Task> Tasks
    {
        get => _tasks;
        set => _tasks = value;
        
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
    
    public List<ProjectRole> ProjectRoles
    {
        get => _projectRoles;
        set => _projectRoles = value;
    }
    
}