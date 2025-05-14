using Backend.Domain;
using Backend.Domain.Enums;
using Task = Backend.Domain.Task;

namespace BackendTest.DomainTest;

[TestClass]
public class NotificationTest
{
    private Notification _notification;
    [TestInitialize]
    public void OnInitialize()
    { 
        _notification = new Notification();
    }
        
    [TestMethod]
    public void CreateNotificationTest()
    {
        Assert.IsNotNull(_notification);
    }
    
    [TestMethod]
    public void AddMessageForNotificationTest()
    {
        _notification.Message = "Notification1";
        Assert.AreEqual("Notification1", _notification.Message);
    }
    
    [TestMethod]
    public void PutMessageNullReturnsAnExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _notification.Message = null);
    }
    
    [TestMethod]
    public void AddDateForNotificationTest()
    {
        DateTime date = DateTime.Now.AddDays(1);
        _notification.Date = date;
        Assert.AreEqual(date, _notification.Date);
    }
    
    [TestMethod]
    public void PutDateInThePastReturnsAnExceptionTest()
    {
        DateTime date = DateTime.Now.AddDays(-1);
        Assert.ThrowsException<ArgumentException>(() => _notification.Date = date);
    }

    [TestMethod]
    public void AddTypeOfNotificationTest()
    {
        _notification.TypeOfNotification = TypeOfNotification.Delay;
        Assert.AreEqual(TypeOfNotification.Delay, _notification.TypeOfNotification);
    }
    
    [TestMethod]
    public void AddImpactForNotificationTest()
    {
        _notification.Impact = 5;
        Assert.AreEqual(5, _notification.Impact);
    }
    
    [TestMethod]
    public void AddNegativeImpactForNotificationTest()
    {
        _notification.Impact = -3;
        Assert.AreEqual(-3, _notification.Impact);
    }    

    [TestMethod]
    public void AddZeroImpactForNotificationReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _notification.Impact = 0);
    }

    [TestMethod]
    public void AddUsersToNotificationTest()
    {
        List<User> users = new List<User>();
        _notification.Users = users;
        Assert.AreEqual(users, _notification.Users);
    }
    
    [TestMethod]
    public void AddNullUsersToNotificationReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _notification.Users = null);
    }
    
    [TestMethod]
    public void AddProjectsToNotificationTest()
    {
        List<Project> projects = new List<Project>();
        _notification.Projects = projects;
        Assert.AreEqual(projects, _notification.Projects);
    }
    
    [TestMethod]
    public void AddNullProjectsToNotificationReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _notification.Projects = null);
    }
    
    [TestMethod]
    public void ViewedBy_ShouldBeEmptyByDefault()
    {
        Assert.IsNotNull(_notification.ViewedBy);
        Assert.AreEqual(0, _notification.ViewedBy.Count);
    }
}