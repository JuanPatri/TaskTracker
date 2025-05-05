using Backend.Domain;

namespace Backend.DTOs.ProjectDTOs;

public class GetProjectDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Project ToEntity()
    {
        return new Project() 
        {
            Id = Id,
            Name = Name
        };
    }
}