namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class ResourceTest
{
    [TestMethod]
    public void CreateResource()
    {
        Resource resource = new Resource();
        Assert.IsNotNull(resource);
    }
}