namespace Backend.DTOs.NotificationDTOs;

public class NotificationDataDTO
{
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public string TypeOfNotification { get; set; } = string.Empty;
    public int Impact { get; set; }
    public List<string> Task { get; set; } = new List<string>();
    public List<string> Users { get; set; } = new List<string>();
    public List<string> Projects { get; set; } = new List<string>();
}