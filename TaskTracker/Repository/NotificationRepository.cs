using Domain;
using Microsoft.Data.SqlClient;

namespace Repository;

public class NotificationRepository : IRepository<Notification>
{
    private readonly SqlContext _sqlContext;
    
    public NotificationRepository(SqlContext sqlContext)
    {
        _sqlContext = sqlContext;
    }

    
    public Notification Add(Notification notification)
    {
        _sqlContext.Notifications.Add(notification);
        _sqlContext.SaveChanges();
        return notification;
    }

    public Notification? Find(Func<Notification, bool> predicate)
    {
        return _sqlContext.Notifications.FirstOrDefault(predicate);
    }

    public IList<Notification> FindAll()
    {
        return _sqlContext.Notifications.ToList();
    }

    public Notification? Update(Notification entity)
    {
        Notification? existingNotification = _sqlContext.Notifications.FirstOrDefault(n => n.Id == entity.Id);
        if (existingNotification != null)
        {
            existingNotification.Id = entity.Id;
            existingNotification.Message = entity.Message;
            existingNotification.Date = entity.Date;
            existingNotification.TypeOfNotification = entity.TypeOfNotification;
            existingNotification.Impact = entity.Impact;
            existingNotification.Users = entity.Users;
            existingNotification.Projects = entity.Projects;
            existingNotification.ViewedBy = entity.ViewedBy;

            _sqlContext.SaveChanges();
            return existingNotification;
        }
        return null;
    }

    public void Delete(string entity)
    {
        Notification? notification = _sqlContext.Notifications.FirstOrDefault(n => n.Message == entity);
        if (notification != null)
        {
            _sqlContext.Remove(notification);
            _sqlContext.SaveChanges();
        }
    }
}