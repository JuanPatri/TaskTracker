namespace Backend.Domain;

public class Project
{
    private string _name { get; set; }

    public string ValidateName
    {
      get => _name;  
      set => _name = value;
    }
}