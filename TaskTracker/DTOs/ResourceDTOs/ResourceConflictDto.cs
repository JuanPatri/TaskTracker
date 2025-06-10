namespace DTOs.ResourceDTOs;

public class ResourceConflictDto
{
    public bool HasConflicts { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> ConflictingTasks { get; set; } = new();
}