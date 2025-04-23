namespace Backend.Domain;

public class Project
{
    private string _name;
    private string _description;

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
        set => _description = value;
    }
    
}