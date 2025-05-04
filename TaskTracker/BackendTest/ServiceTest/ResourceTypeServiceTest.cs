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
    
    [TestMethod]
    public void AddResourceTypeShouldReturnResource()
    {
        // ResourceDataDto resource = new ResourceDataDto()
        // {
        //     Name = "name",
        //     Description = "description",
        //     TypeResource = 1
        // };
        //
        // Resource? createdResource = _resourceService.AddResource(resource);
        // Assert.IsNotNull(createdResource);
        // Assert.AreEqual(_resourceRepository.FindAll().Last(), createdResource);
    }
}