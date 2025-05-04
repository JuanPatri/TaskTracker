using Backend.Domain;
using Backend.DTOs.ResourceDTOs;
using Backend.Repository;
using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class ResourceServiceTest
{
    private ResourceRepository _resourceRepository;
    private ResourceTypeRepository _resourceTypeRepository;
    private ResourceService _resourceService;
    private Resource _resource;
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceRepository = new ResourceRepository(); 
        _resourceTypeRepository = new ResourceTypeRepository();
        _resourceService = new ResourceService(_resourceRepository, _resourceTypeRepository);
        _resource = new Resource()
        {
            Name = "Resource",
            Description = "Description",
            Type = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            }
        };
        _resourceRepository.Add(_resource);
        _resourceTypeRepository.Add(_resource.Type);
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
            TypeResource = 1
        };
        
        Resource? createdResource = _resourceService.AddResource(resource);
        Assert.IsNotNull(createdResource);
        Assert.AreEqual(_resourceRepository.FindAll().Last(), createdResource);
    }
    
    [TestMethod]
    public void RemoveResourceShouldRemoveResource()
    {
        Assert.AreEqual(_resourceRepository.FindAll().Last(), _resource);   

        ResourceDataDto resourceToDelete = new ResourceDataDto()
        {
            Name = "Resource",
            Description = "Description",
            TypeResource = 4
        };
        _resourceService.RemoveResource(resourceToDelete);
        
        Assert.AreNotEqual(_resourceRepository.FindAll().Last(), _resource);
    }
}