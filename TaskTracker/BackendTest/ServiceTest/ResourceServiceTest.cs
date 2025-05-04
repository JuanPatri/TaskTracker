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
        Assert.AreEqual(_resourceRepository.FindAll().Count, 1);

        GetResourceDto resourceToDelete = new GetResourceDto()
        {
            Name = "Resource"
        };
            
        _resourceService.RemoveResource(resourceToDelete);
        
        Assert.AreEqual(_resourceRepository.FindAll().Count, 0);
    }
    
    [TestMethod]
    public void GetResourceReturnResource()
    {
        GetResourceDto resourceToFind = new GetResourceDto()
        {
            Name = "Resource"
        };
        
        Assert.AreEqual(_resourceService.GetResource(resourceToFind), _resource);
    }
    
    [TestMethod]
    public void GetAllResourceReturnAllResource()
    {
        Resource newResource = new Resource()
        {
            Name = "Resource2",
            Description = "Description",
            Type = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            }
        };
        _resourceRepository.Add(newResource);
        List <Resource> resources = _resourceService.GetAllResource();
        
        Assert.IsTrue(resources.Any(r => r.Name == "Resource"));
        Assert.IsTrue(resources.Any(r => r.Name == "Resource2"));
    }
}