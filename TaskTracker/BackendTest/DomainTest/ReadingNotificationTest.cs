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
        User user = new User();
        _readingNotification.User = user;
        Assert.AreEqual(user, _readingNotification.User);
    }
}