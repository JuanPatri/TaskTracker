using Backend.Domain;
using Backend.DTOs.ResourceDTOs;
using Backend.Repository;
using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class ResourceServiceTest
{
    private ResourceRepository _resourceRepository;
    private ResourceService _resourceService;
    private Resource _resource;
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceRepository = new ResourceRepository();
        _resourceService = new ResourceService(_resourceRepository);
        _resource = new Resource()
        {
            Name = "Resource",
            Description = "Description",
        };
    }
    
    [TestMethod]
    public void CreateResourceService()
    {
        Assert.IsNotNull(_resourceService);
    }
    
    [TestMethod]
    public void AddResourceShouldReturnResource()
    {
        ResourceDataDto resource = new ResourceDataDto()
        {
            Name = "name",
            Description = "description",
            TypeResource = "Type"
        };
        Resource? createdResource = _resourceService.AddResource(resource);
        Assert.IsNotNull(createdResource);
        Assert.AreEqual(_resourceRepository.FindAll().Last(), createdResource);
    }
}