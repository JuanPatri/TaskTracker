using Backend.Domain;

namespace BackendTest.DomainTest;

[TestClass]
public class ReadingNotificationTest
{
    private ReadingNotification _readingNotification;

    [TestInitialize]
    public void OnInitialize()
    {
        _readingNotification = new ReadingNotification();
    }
    
    [TestMethod]
    public void CreateNotificationTest()
    {
       Assert.IsNotNull(_readingNotification); 
    }
    
    [TestMethod]
    public void SetUserForReadingNotificationTest()
    {
        List<User> user = new List<User>();
        _readingNotification.User = user;
        Assert.AreEqual(user, _readingNotification.User);
    }
    
    [TestMethod]
    public void SetUserNullReturnsAnExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _readingNotification.User = null);
    }
    
    [TestMethod]
    public void SetNotificationForReadingNotificationTest()
    {
        List<Notification> notification = new List<Notification>();
        _readingNotification.Notification = notification;
        Assert.AreEqual(notification, _readingNotification.Notification);
    }
    
    [TestMethod]
    public void SetNotificationNullReturnsAnExceptionTest()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _readingNotification.Notification = null);
    }

    [TestMethod]
    public void SetFueLeidaForReadingNotificationTest()
    {
        _readingNotification.Read = false;
        Assert.IsFalse(_readingNotification.Read);
    }
}