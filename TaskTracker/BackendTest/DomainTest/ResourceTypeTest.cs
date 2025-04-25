namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class ResourceTypeTest
{
    [TestMethod]
    public void CreateType()
    {
        ResourceType _type = new ResourceType();
        Assert.IsNotNull(_type);
    }
}