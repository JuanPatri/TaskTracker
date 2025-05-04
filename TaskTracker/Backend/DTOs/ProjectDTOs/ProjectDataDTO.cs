namespace Backend.DTOs.ProjectDTOs;

public class ProjectDataDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
}