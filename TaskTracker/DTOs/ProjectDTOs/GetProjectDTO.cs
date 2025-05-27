using DTOs.TaskDTOs;

namespace DTOs.ProjectDTOs;

public class GetProjectDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EstimatedFinish { get; set; }
    public List<GetTaskDTO> Tasks { get; set; } = new();
    public List<string> CriticalPathTitles { get; set; } = new();
    public DateOnly StartDate { get; set; }
}