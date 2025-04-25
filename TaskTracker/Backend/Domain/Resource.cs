namespace Backend.Domain;

public class Resource
{
    private string _name;
    
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The resource name cannot be empty");
            _name = value;
        }
    }
}