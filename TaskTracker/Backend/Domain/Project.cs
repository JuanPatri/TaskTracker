namespace Backend.Domain;

public class Project
{
    private string _name;
    private string _description;
    private DateTime _startDate;

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
}