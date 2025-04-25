namespace Backend.Domain;

public class Resource
{
    private string _name;
    private string _description;
    private ResourceType _type;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The resource name cannot be empty");
            _name = value;
        }
    }
    
    public string Description
    {
        get => _description;
        set => _description = value;
    }
    
    public ResourceType Type
    {
        get => _type;
        set
        {
            if (value == null) throw new ArgumentException("Resource type cannot be null");
            _type = value;
        }
    }
}