using Backend.Domain;
using Backend.DTOs.TaskDTOs;

namespace Backend.DTOs.ProjectDTOs;

public class GetProjectDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EstimatedFinish { get; set; }
    public List<GetTaskDTO> Tasks { get; set; } = new();
    public List<string> CriticalPathTitles { get; set; } = new();
    
    public Project ToEntity()
    {
        return new Project() 
        {
            Id = Id,
            Name = Name
        };
    }
}