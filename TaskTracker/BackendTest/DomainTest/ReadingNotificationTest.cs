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
}