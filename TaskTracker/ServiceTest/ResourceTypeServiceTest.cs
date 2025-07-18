using Domain;
using DTOs.ResourceTypeDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTest.Context;
using Service;

namespace ServiceTest;

[TestClass]
public class ResourceTypeServiceTest
{
    private IRepository<ResourceType> _resourceTypeRepository;
    private ResourceTypeService _resourceTypeService;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _resourceTypeRepository = new ResourceTypeRepository(_sqlContext);
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
    public void AddResourceTypeShouldThrowExceptionIfResourceAlreadyExists()
    {
        ResourceTypeDto resourceType = new ResourceTypeDto()
        {
            Id = 4,
            Name = "Human"
        };

        Assert.ThrowsException<Exception>(() => _resourceTypeService.AddResourceType(resourceType));
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
        List<ResourceType> resourcesTypes = _resourceTypeService.GetAllResourcesType();

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