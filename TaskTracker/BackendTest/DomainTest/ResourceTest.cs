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

    [TestMethod]
    public void PutNullNameToResource()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _resource.Name = null);
        Assert.AreEqual("The resource name cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void PutDescriptionToResource()
    {
        string expectedDescription = "Valid Description";
        _resource.Description = expectedDescription;
        Assert.AreEqual(expectedDescription, _resource.Description);
    }
    
    
}