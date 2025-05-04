using Backend.Domain;

namespace Backend.DTOs.ResourceTypeDTOs;

public class ResourceTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ResourceType ToEntity()
    {
        return new ResourceType()
        {
            Id = Id,
            Name = Name
        };
    }
}