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
    public void RemoveResourceTypeShouldRemoveResourceType()
    {
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 3);

        ResourceTypeDto resourceToDelete = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Human",
        };
        
        _resourceTypeService.RemoveResourceType(resourceToDelete);
        
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 2);
    }
    
    [TestMethod]
    public void GetResourceTypeReturnResourceType()
    {
        ResourceTypeDto resourceToFind = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Resource"
        };
        
        Assert.AreEqual(_resourceTypeService.GetResourceType(resourceToFind).Name, "Human");
    }
    
    [TestMethod]
    public void GetAllResourceTypeReturnAllResourceType()
    {
        List <ResourceType> resourcesTypes = _resourceTypeService.GetAllResourcesType();
        
        Assert.IsTrue(resourcesTypes.Any(r => r.Name == "Human"));
        Assert.IsTrue(resourcesTypes.Any(r => r.Name == "Software"));
    }
    
    [TestMethod]
    public void UpdateResourceTypeShouldModifyResourceTypeData()
    {
        Assert.AreEqual(_resourceTypeRepository.Find(r => r.Id == 1).Name, "Human");

        ResourceTypeDto resourceTypeDTO = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Resource"
        };
        
        _resourceTypeService.UpdateResourceType(resourceTypeDTO);
        Assert.AreEqual(_resourceTypeRepository.Find(r => r.Id == 1).Name, "Resource");
    }
}