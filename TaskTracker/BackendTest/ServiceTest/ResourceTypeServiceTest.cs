using Backend.Domain;
using Backend.Repository;
using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class ResourceTypeServiceTest
{
    private ResourceTypeRepository _resourceTypeRepository;
    private ResourceTypeService _resourceTypeService;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceTypeRepository = new ResourceTypeRepository();
        _resourceTypeService = new ResourceTypeService(_resourceTypeRepository);
    }
    
    [TestMethod]
    public void CreateResourceTypeService()
    {
        Assert.IsNotNull(_resourceTypeService);
    }
}