namespace BackendTest.DomainTest;

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