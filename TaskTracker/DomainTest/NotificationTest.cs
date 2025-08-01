using Domain;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTest;

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
    public void ViewedByShouldBeEmptyByDefault()
    {
        Assert.IsNotNull(_notification.ViewedBy);
        Assert.AreEqual(0, _notification.ViewedBy.Count);
    }
    
    [TestMethod]
    public void AddEmailToViewedByTest()
    {
        _notification.ViewedBy.Add("user@mail.com");
        Assert.IsTrue(_notification.ViewedBy.Contains("user@mail.com"));
    }
    
    [TestMethod]
    public void SetViewedByToNullShouldSetEmptyList()
    {
        _notification.ViewedBy = null;
        Assert.IsNotNull(_notification.ViewedBy);
        Assert.AreEqual(0, _notification.ViewedBy.Count);
    }
    
    [TestMethod]
    public void SetValidIdShouldAssignId()
    {
        var notification = new Notification();
        notification.Id = 1;
        Assert.AreEqual(1, notification.Id);
    }
    
    [TestMethod]
    public void SetZeroIdShouldThrowException()
    {
        var notification = new Notification();
        Assert.ThrowsException<ArgumentException>(() => notification.Id = 0);
    }
    
    [TestMethod]
    public void SetNegativeIdShouldThrowException()
    {
        var notification = new Notification();
        Assert.ThrowsException<ArgumentException>(() => notification.Id = -5);
    }
    
    [TestMethod]
    public void MultipleNotificationsShouldHaveDifferentIds()
    {
        var n1 = new Notification();
        var n2 = new Notification();
        n1.Id = 1;
        n2.Id = 2;
        Assert.AreNotEqual(n1.Id, n2.Id);
    }
}