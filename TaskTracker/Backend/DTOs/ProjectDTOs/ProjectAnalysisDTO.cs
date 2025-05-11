namespace Backend.DTOs.ProjectDTOs;

public class ProjectAnalysisDTO
{
    public DateTime EstimatedFinishDate { get; set; } 

    public List<string> CriticalTaskTitles { get; set; } = new();  

    public Dictionary<string, double> NonCriticalTaskSlack { get; set; } = new();  
}
