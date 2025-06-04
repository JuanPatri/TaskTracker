using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTest;

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
    
    [TestMethod]
    public void AddResourceTypeToResource()
    {
        ResourceType _type = new ResourceType();
        _resource.Type = _type;
        Assert.AreEqual(_type, _resource.Type);
    }
    
    [TestMethod]
    public void AddNullResourceTypeToResource()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _resource.Type = null);
        Assert.AreEqual("ResourceDTOs type cannot be null", ex.Message);
    }
    
    [TestMethod]
    public void SetIdForResourceTest()
    {
        _resource.Id = 1;
        Assert.AreEqual(1, _resource.Id);
    }
    
    [TestMethod]
    public void SetIdNegativeReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _resource.Id = -1);
    }
    
    [TestMethod]
    public void SetQuiantityForResourceTest()
    {
        _resource.Quantity = 5;
        Assert.AreEqual(5, _resource.Quantity);
    }
    
    [TestMethod]
    public void SetQuantityNegativeReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _resource.Quantity = -1);
    }
}