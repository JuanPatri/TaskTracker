using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTest;

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
        _type.Id = 1;
        Assert.AreEqual(1, _type.Id);
        Assert.AreEqual("Pedro", _type.Name);
    }
    
    [TestMethod]
    public void PutNameNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _type.Name = null);
        Assert.AreEqual("The name cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void PutIdLessThanOrEqualToZeroReturnsAnException()
    {
        Assert.ThrowsException<ArgumentException>(() => _type.Id = 0);
    }
}