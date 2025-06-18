using DTOs.ResourceDTOs;

namespace Domain;
public class Resource
{
    private int _id;
    private string _name;
    private string _description;
    private ResourceType _type;
    private int _quantity;
    private int? _projectId;
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
            if (value == null) throw new ArgumentException("ResourceDTOs type cannot be null");
            _type = value;
        }
    }
    
    public int Id
    {
        get => _id;
        set
        {
            if (value <= 0) throw new ArgumentException("Id must be greater than 0");
            _id = value;
        }
    }
    
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0) throw new ArgumentException ("Quantity cannot be negative");
            _quantity = value; 
            
        }
    }
    
    public int? ProjectId
    {
        get => _projectId;
        set => _projectId = value;
    }
}