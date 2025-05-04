using Backend.Domain;
using Backend.DTOs.ResourceTypeDTOs;
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
        ResourceTypeDto resourceType = new ResourceTypeDto()
        {
            Id = 4,
            Name = "name" 
        };
        
        ResourceType? createdResourceType = _resourceTypeService.AddResourceType(resourceType);
        Assert.IsNotNull(createdResourceType);
        Assert.AreEqual(_resourceTypeRepository.FindAll().Last(), createdResourceType);
    }
    
    [TestMethod]
    public void RemoveResourceTypeShouldRemoveResource()
    {
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 3);

        ResourceTypeDto resourceToDelete = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Human",
        };
        
        _resourceTypeRepository.RemoveResourceType(resourceToDelete);
        
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 2);
    }
}