using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class ResourceServiceTest
{
    private ResourceService _resourceService;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceService = new ResourceService();
    }
    
    [TestMethod]
    public void CreateResourceService()
    {
        Assert.IsNotNull(_resourceService);
    }
}