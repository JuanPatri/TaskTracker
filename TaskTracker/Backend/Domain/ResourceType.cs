namespace Backend.Domain;

public class ResourceType
{
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The name cannot be empty");
            _name = value;
        }
    }
}