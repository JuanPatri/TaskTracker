using Backend.Domain;
using Backend.Domain.Enums;

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
        _notification.ValidateTypeOfNotification = TypeOfNotification.Delay;
        Assert.AreEqual(TypeOfNotification.Delay, _notification.ValidateTypeOfNotification);
    }
    
    [TestMethod]
    public void AddImpactForNotificationTest()
    {
        _notification.Impact = 5;
        Assert.AreEqual(5, _notification.Impact);
    }

    [TestMethod]
    public void AddZeroOrNegativeImpactForNotificationReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _notification.Impact = 0);
    }
}