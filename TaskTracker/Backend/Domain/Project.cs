namespace Backend.Domain;

public class Project
{
    private string _name { get; set; }

    public string ValidateName
    {
      get => _name;  
      set
      {
          if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The project name cannot be empty");
          _name = value;
      }
    }
    
    
}