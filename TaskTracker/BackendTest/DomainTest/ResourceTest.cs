namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class ResourceTest
{
    private Resource _resource;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _resource = new Resource();
    }
    
    [TestMethod]
    public void CreateResource()
    {
        Assert.IsNotNull(_resource);
    }
    
    [TestMethod]
    public void PutValidNameToResource()
    {
        string expectedName = "Valid Name";
        _resource.Name = expectedName;
        Assert.AreEqual(expectedName, _resource.Name);
    }

    
}