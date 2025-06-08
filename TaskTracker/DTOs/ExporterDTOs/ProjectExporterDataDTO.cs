namespace DTOs.ExporterDTOs;

public class ProjectExporterDataDto
{
    public string Name { get; set; } = String.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public List<TaskExporterDataDto> Tasks { get; set; } = new List<TaskExporterDataDto>();
}