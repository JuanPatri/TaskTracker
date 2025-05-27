using Domain.Enums;

namespace DTOs.NotificationDTOs;

public class NotificationDataDTO
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public TypeOfNotification TypeOfNotification { get; set; }
    public int Impact { get; set; }
    public List<string> Task { get; set; } = new List<string>();
    public List<string> Users { get; set; } = new List<string>();
    public List<string> Projects { get; set; } = new List<string>();
    public List<string> ViewedBy { get; set; } = new List<string>();    
    
    public Notification ToEntity()
    {
        return new Notification
        {
            Id = Id,
            Message = Message
        };
    }
}