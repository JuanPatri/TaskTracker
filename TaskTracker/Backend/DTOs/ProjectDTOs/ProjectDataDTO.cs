using Backend.Domain;
using Backend.DTOs.UserDTOs;

namespace Backend.DTOs.ProjectDTOs;

public class ProjectDataDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly FinishDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public UserDataDTO Administrator { get; set; } = new UserDataDTO();
    public List<string> Users { get; set; } = new List<string>();

}