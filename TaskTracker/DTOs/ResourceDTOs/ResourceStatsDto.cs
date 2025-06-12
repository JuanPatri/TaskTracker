namespace DTOs.ResourceDTOs;

public class ResourceStatsDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public string TaskName { get; set; } = string.Empty;
    public int UsageLevel { get; set; }
    public string UsagePeriod { get; set; } = string.Empty;
}