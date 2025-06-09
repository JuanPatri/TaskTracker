namespace DTOs.ExporterDTOs;

public class TaskExporterDataDto
{
    public string Title { get; set; } = String.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public int Duration { get; set; } = 0;
    public string IsCritical { get; set; } = "N";
    public List<string> Resources { get; set; } = new List<string>();
}