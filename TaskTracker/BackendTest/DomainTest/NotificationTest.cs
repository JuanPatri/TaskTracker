using Backend.Domain;

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
        DateTime date = DateTime.Now;
        _notification.Date = date;
        Assert.AreEqual(date, _notification.Date);
    }
}