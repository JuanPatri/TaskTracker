namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class UserTest
{
    private User user;
    
    [TestInitialize]
    public void OnInitialize()
    {
        user = new User();
    }
    
    [TestMethod]
    public void CreateUser()
    {
        Assert.IsNotNull(user);
    }
    
    [TestMethod]
    public void CreateNameForUser()
    {
        user.name = "Pedro";
        Assert.AreEqual("Pedro", user.name);
    }
}