using Backend.DTOs.ProjectDTOs;

namespace Backend.Domain;

public class Project
{
    private int _id;
    private string _name;
    private string _description;
    private DateTime _startDate;
    private DateTime _finishDate;
    private User _administrator;
    private List<Task> _tasks;

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
    public DateTime StartDate 
    {
        get => _startDate;
        set 
        {
            if (value < DateTime.Now) throw new ArgumentException("The project start date cannot be in the past");
            _startDate = value;
        }
    }
    
    public DateTime FinishDate 
    {
        get => _finishDate;
        set
        {
            if (value < _startDate) throw new ArgumentException("The finish date cannot be before the start date");
            _finishDate = value;
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
    
    public static Project FromDto(ProjectDataDTO projectDataDto)
    {
        return new Project()
        {
            Id = projectDataDto.Id,
            Name = projectDataDto.Name,
            Description = projectDataDto.Description,
            StartDate = projectDataDto.StartDate,
            FinishDate = projectDataDto.FinishDate,
            Administrator = User.FromDto(projectDataDto.Administrator)
        };
    }
}