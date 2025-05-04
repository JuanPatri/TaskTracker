using Backend.DTOs.UserDTOs;

namespace Backend.DTOs.ProjectDTOs;

public class ProjectDataDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime FinishDate { get; set; } = DateTime.Today;
    public UserDataDTO Administrator { get; set; } = new UserDataDTO();
}