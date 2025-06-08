namespace DTOs.ExporterDTOs;

public class ProjectExporterDataDto
{
    public string Name { get; set; } = String.Empty;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public List<TaskExporterDataDto> Tasks { get; set; } = new List<TaskExporterDataDto>();
}