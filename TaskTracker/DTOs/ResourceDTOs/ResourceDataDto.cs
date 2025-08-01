namespace DTOs.ResourceDTOs;

public class ResourceDataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TypeResource { get; set; }
    public int Quantity { get; set; }
}