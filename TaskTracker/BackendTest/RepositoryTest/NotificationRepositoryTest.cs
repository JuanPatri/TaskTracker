using Backend.Domain;
using Backend.Domain.Enums;
using Backend.Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class NotificationRepositoryTest
{
    private NotificationRepository _notificationRepository;
    private Notification _notification;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _notificationRepository = new NotificationRepository();
        _notification = new Notification();
        _notification.Message = "new notification";
    }
    
    [TestMethod]
    public void CreateNotificationRepositoryTest()
    {
        Assert.IsNotNull(_notificationRepository);
    }

}