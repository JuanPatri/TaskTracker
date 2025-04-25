namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class ResourceTypeTest
{
    private ResourceType _type;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _type = new ResourceType();
    }
    
    [TestMethod]
    public void CreateUser()
    {
        Assert.IsNotNull(_type);
    }
    
    [TestMethod]
    public void CreateNameForUser()
    {
        _type.Name = "Pedro";
        Assert.AreEqual("Pedro", _type.Name);
    }
}