namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class UserTest
{
    private User _user;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _user = new User();
    }
    
    [TestMethod]
    public void CreateUser()
    {
        Assert.IsNotNull(_user);
    }
    
    [TestMethod]
    public void CreateNameForUser()
    {
        _user.Name = "Pedro";
        Assert.AreEqual("Pedro", _user.Name);
    }
    
    [TestMethod]
    public void IPutNameNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.Name = null);
        Assert.AreEqual("The username cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void CreateLastNameForUser()
    {
        _user.LastName = "Rodriguez";
        Assert.AreEqual("Rodriguez", _user.LastName);
    }
    
}