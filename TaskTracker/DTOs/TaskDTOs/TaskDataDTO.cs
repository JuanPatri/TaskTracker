using DTOs.TaskResourceDTOs;
using Enums;

namespace DTOs.TaskDTOs;

public class TaskDataDTO
{ 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; } = 1;
    public Status Status { get; set; } = Status.Pending;
    public List<String> Dependencies { get; set; } = new List<string>();
    public List<TaskResourceDataDTO> Resources { get; set; } = new List<TaskResourceDataDTO>();
    public DateTime? DateCompleated { get; set; } = null;
}