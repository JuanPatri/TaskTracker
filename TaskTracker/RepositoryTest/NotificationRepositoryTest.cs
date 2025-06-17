using BusinessLogicTest.Context;
using Domain;
using Enums;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;

namespace RepositoryTest;

[TestClass]
public class NotificationRepositoryTest
{
    private NotificationRepository _notificationRepository;
    private Notification _notification;
    private SqlContext _sqlContext;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _notificationRepository = new NotificationRepository(_sqlContext);
        _notification = new Notification();
        _notification.Id = 1;
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
    
    [TestMethod]
    public void UpdateExistingNotificationUpdatesFieldsCorrectlyTest()
    {
        _notification.Message = "UpdatedMessage";
        _notificationRepository.Add(_notification);
        Notification updateNotification = new Notification()
        {
            Id = _notification.Id,
            Message = "UpdatedMessage",
            Date = DateTime.Now.AddSeconds(5),
            TypeOfNotification = TypeOfNotification.Delay,
            Impact = 2,
            Users = new List<User>(),
            Projects = new List<Project>(),
            ViewedBy = new List<string>()
        };
        _notificationRepository.Update(updateNotification);
        Assert.AreEqual(_notification.Message, "UpdatedMessage");
    }

    [TestMethod]
    public void UpdateNonExistingNotificationReturnsNullTest()
    {
        Notification updateNotification = new Notification()
        {
            Id = 999,
            Message = "NonExistingMessage",
            Date = DateTime.Now.AddSeconds(5),
            TypeOfNotification = TypeOfNotification.Delay,
            Impact = 2,
            Users = new List<User>(),
            Projects = new List<Project>(),
            ViewedBy = new List<string>()
        };
        Assert.IsNull(_notificationRepository.Update(updateNotification));
    }
    
    [TestMethod]
    public void DeleteNotificationTest()
    {
        _notificationRepository.Add(_notification);
        Assert.AreEqual(_notificationRepository.FindAll().Count, 1);
        _notificationRepository.Delete(_notification.Message);
        Assert.AreEqual(_notificationRepository.FindAll().Count, 0);
    }
}