using Backend.Domain;

namespace Backend.Repository;

public class NotificationRepository : IRepository<Notification>
{
    private readonly List<Notification> _notifications;
    
    public NotificationRepository()
    {
        _notifications = new List<Notification>();
    }
    
    public Notification Add(Notification notification)
    {
        _notifications.Add(notification);
        return notification;
    }

    public Notification? Find(Func<Notification, bool> predicate)
    {
        return _notifications.FirstOrDefault(predicate);
    }

    public IList<Notification> FindAll()
    {
        return _notifications;
    }

    public Notification? Update(Notification entity)
    {
        Notification? existingNotification = _notifications.FirstOrDefault(n => n.Message == entity.Message);
        if (existingNotification != null)
        {
            existingNotification.Message = entity.Message;
            existingNotification.Date = entity.Date;
            existingNotification.TypeOfNotification = entity.TypeOfNotification;
            existingNotification.Impact = entity.Impact;
            existingNotification.Users = entity.Users;
            existingNotification.Projects = entity.Projects;

            return existingNotification;
        }
        return null;
    }

    public void Delete(string entity)
    {
        Notification? notification = _notifications.FirstOrDefault(n => n.Message == entity);
        if (notification != null)
        {
            _notifications.Remove(notification);
        }
    }
}