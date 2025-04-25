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
}