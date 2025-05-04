using Backend.Domain;

namespace Backend.DTOs.ResourceDTOs;

public class GetResourceDto
{
    public string Name { get; set; } = string.Empty;

    
    public Resource ToEntity()
    {
        return new Resource()
        {
            Name = Name
        };
    }
}