namespace Backend.Domain;

public class Proyect
{
    private string _name { get; set; }

    public string ValidateName
    {
      get => _name;  
      set => _name = value;
    }
}