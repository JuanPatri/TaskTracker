using Backend.Domain;

namespace Backend.DTOs.ProjectDTOs;

public class GetProjectDTO
{
    public string Name { get; set; } = string.Empty;
    
    public Project ToEntity()
    {
        return new Project() 
        {
            Name = Name
        };
    }
}