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
    
    [TestMethod]
    public void AddNotificationToListTest()
    {
        _notificationRepository.Add(_notification);
        Assert.AreEqual(_notificationRepository.Find(n => n.Message == "new notification"), _notification);
    }
    
    [TestMethod]
    public void SearchForAllNotificationInTheListTest()
    {
        Assert.AreEqual(_notificationRepository.FindAll().Count, 0);
        _notificationRepository.Add(_notification);
        Assert.AreEqual(_notificationRepository.FindAll().Count, 1);
    }

}