using Backend.Domain;

namespace Backend.DTOs.ResourceDTOs;

public class ResourceDataDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TypeResource { get; set; }
    
}