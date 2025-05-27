using DTOs.ResourceTypeDTOs;

namespace Domain;

public class ResourceType
{
    private int _id;
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
    
    public int Id
    {
        get => _id;
        set
        {
            if (value <= 0) throw new ArgumentException("The id cannot be less than or equal to 0");
            _id = value;
        }
    }
    
    public static ResourceType Fromdto(ResourceTypeDto resourceTypeDto)
    {
        return new ResourceType()
        {
            Id = resourceTypeDto.Id,
            Name = resourceTypeDto.Name
        };
    }
}