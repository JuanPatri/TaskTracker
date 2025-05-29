using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ResourceTypeRepositoryTest
{
    private ResourceTypeRepository _resourceTypeRepository;
    private ResourceType _resourceType;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceTypeRepository = new ResourceTypeRepository();
        _resourceType = new ResourceType();
        _resourceType.Name = "new resource type";
    }
    [TestMethod]
    public void CreateResourceRepository()
    { 
        Assert.IsNotNull(_resourceTypeRepository);
    }
    
    [TestMethod]
    public void AddResourceTypeToRepository()
    {
        _resourceType.Id = 4;
        _resourceTypeRepository.Add(_resourceType);
        Assert.AreEqual(_resourceTypeRepository.Find(r => r.Id == 4), _resourceType);
    }
    
    [TestMethod]
    public void SearchForAllResourceTypeInTheList()
    {
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 3);
        _resourceTypeRepository.Add(_resourceType);
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 4);
    }
    
    [TestMethod]
    public void UpdateExistingResourceTypeUpdatesFieldsCorrectly()
    {
        _resourceType.Id = 4;
        _resourceTypeRepository.Add(_resourceType);
        Assert.AreEqual(_resourceType.Id, 4);
        
        ResourceType updateResourceType = new ResourceType()
        {
            Id = 4,
            Name = "resource type"
        };
        
        _resourceTypeRepository.Update(updateResourceType);
        Assert.AreEqual(_resourceType.Name, "resource type");
    }
    
    [TestMethod]
    public void UpdatingAResourceTypeThatIsNotInTheListReturnsNull()
    {
        _resourceTypeRepository.Add(_resourceType);
        ResourceType updateResourceType = new ResourceType()
        {
            Id = 5,
            Name = "UpdatedName"
        };
        
        
        Assert.AreEqual(_resourceTypeRepository.Update(updateResourceType), null);
    }
    
    [TestMethod]
    public void DeleteResourceTypeFromList()
    {
        _resourceTypeRepository.Add(_resourceType);
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 4);
        _resourceTypeRepository.Delete(_resourceType.Id.ToString());
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 3);
    }
}