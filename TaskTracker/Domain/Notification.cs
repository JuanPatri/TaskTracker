using Backend.Domain;
using Backend.Domain.Enums;
using DTOs.NotificationDTOs;

namespace Domain;
public class Notification
{
    private int _id;
    private string _message;
    private DateTime _date;
    private TypeOfNotification _typeOfNotification;
    private int _impact;
    private List<User> _users;
    private List<Project> _projects;
    private List<string> _viewedBy = new List<string>();
    
    private const int Zero = 0;
    public string Message
    {
        get => _message;
        set 
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("The notification message cannot be empty");
            _message = value;
        } 
    }
    
    public DateTime Date
    {
        get => _date;
        set 
        {
            if (value < DateTime.Now) throw new ArgumentException("The notification date cannot be in the past");
            _date = value;
        }
    }
    
    public TypeOfNotification TypeOfNotification
    {
        get => _typeOfNotification;
        set => _typeOfNotification = value;
    }
    
    public int Impact
    {
        get => _impact;
        set
        {
            if (value == Zero) throw new ArgumentException("The impact can't be 0");
            _impact = value;
        }
    }
    public List<User> Users
    {
        get => _users;
        set
        {
            if (value == null) throw new ArgumentNullException("The users cannot be null");
            _users = value;
        }
    }
    public List<Project> Projects
    {
        get => _projects;
        set
        {
            if (value == null) throw new ArgumentNullException("The projects cannot be null");
            _projects = value;
        }
    }
    
    public List<string> ViewedBy
    {
        get => _viewedBy;
        set => _viewedBy = value ?? new List<string>();
    }
    
    public int Id
    {
        get => _id;
        set
        {
            if (value <= 0) throw new ArgumentException("El Id de la notificación debe ser positivo");
            _id = value;
        }
    }

    public static Notification FromDto(NotificationDataDTO notificationDataDto, List<User> users, List<Project> projects, List<string> viewedBy)
    {
        return new Notification()
        {
            Id = notificationDataDto.Id,
            Message = notificationDataDto.Message,
            Date = notificationDataDto.Date,
            TypeOfNotification = notificationDataDto.TypeOfNotification,
            Impact = notificationDataDto.Impact,
            Users = users,
            Projects = projects,
            ViewedBy = viewedBy
        };
    }
}