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
        throw new NotImplementedException();
    }

    public Notification? Update(Notification entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(string entity)
    {
        throw new NotImplementedException();
    }
}