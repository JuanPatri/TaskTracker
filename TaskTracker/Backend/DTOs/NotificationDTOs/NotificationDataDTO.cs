using Backend.Domain;
using Backend.Domain.Enums;
using Task = Backend.Domain.Task;

namespace Backend.DTOs.NotificationDTOs;

public class NotificationDataDTO
{
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public TypeOfNotification TypeOfNotification { get; set; }
    public int Impact { get; set; }
    public List<string> Task { get; set; } = new List<string>();
    public List<string> Users { get; set; } = new List<string>();
    public List<string> Projects { get; set; } = new List<string>();
    
    public Notification ToEntity()
    {
        return new Notification
        {
            Message = Message,
        };
    }
}