namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class UserTest
{
    private User _user;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _user = new User("jua");
    }
    
    [TestMethod]
    public void CreateUser()
    {
        Assert.IsNotNull(_user);
    }
    
    [TestMethod]
    public void CreateNameForUser()
    {
        User user2 = new User("Pedro");
        Assert.AreEqual("Pedro", user2.getName());
    }
}