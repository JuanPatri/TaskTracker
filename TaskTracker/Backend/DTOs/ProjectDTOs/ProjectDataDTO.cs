using Backend.Domain;
using Backend.DTOs.UserDTOs;

namespace Backend.DTOs.ProjectDTOs;

public class ProjectDataDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime FinishDate { get; set; } = DateTime.Today;
    public UserDataDTO Administrator { get; set; } = new UserDataDTO();
    
    public Project ToEntity()
    {
        return new Project()
        {
            Name = Name,
            Description = Description,
            StartDate = StartDate,
            FinishDate = FinishDate,
            Administrator = Administrator.ToEntity()
        };
    }
}