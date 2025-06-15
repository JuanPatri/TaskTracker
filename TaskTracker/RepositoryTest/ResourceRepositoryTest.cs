using BusinessLogicTest.Context;
using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ResourceRepositoryTest
{
    private ResourceRepository _resourceRepository;
    private Resource _resource;
    private SqlContext _sqlContext;
    private ResourceType _resourceType;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _resourceRepository = new ResourceRepository(_sqlContext);
        
        _resourceType = new ResourceType
        {
            Name = "Test Type"
        };
        
        _sqlContext.ResourceTypes.Add(_resourceType);
        _sqlContext.SaveChanges();
        
        _resource = new Resource
        {
            Name = "new resource",
            Description = "Test description",  
            Quantity = 10,                     
            Type = _resourceType
        };
        
    }
    [TestMethod]
    public void CreateResourceRepository()
    { 
        Assert.IsNotNull(_resourceRepository);
    }
    
    [TestMethod]
    public void AddResourceToRepository()
    {
        _resourceRepository.Add(_resource);
        Resource foundResource = _resourceRepository.Find(r => r.Name == "new resource");
        Assert.IsNotNull(foundResource);
        Assert.AreEqual("new resource", foundResource.Name);
    }
    
    [TestMethod]
    public void SearchForAllResourceInTheList()
    {
        Assert.AreEqual(0, _resourceRepository.FindAll().Count);

        _resourceRepository.Add(_resource);

        Assert.AreEqual(1, _resourceRepository.FindAll().Count);
    }
    
    [TestMethod]
    public void UpdateExistingResourceUpdatesFieldsCorrectly()
    {
        _resource.Description = "Description";
        Resource addedResource = _resourceRepository.Add(_resource);
        Assert.AreEqual("Description", addedResource.Description);
        
        ResourceType updateResourceType = new ResourceType
        {
            Name = "UpdatedType"
        };
        _sqlContext.ResourceTypes.Add(updateResourceType);
        _sqlContext.SaveChanges();
        
        Resource updateResource = new Resource
        {
            Id = addedResource.Id, 
            Name = "new resource",
            Description = "UpdatedDescription",
            Quantity = 10,
            Type = updateResourceType
        };
        
        Resource updatedResource = _resourceRepository.Update(updateResource);
        Assert.IsNotNull(updatedResource);
        Assert.AreEqual("UpdatedDescription", updatedResource.Description);
    }
    
    [TestMethod]
    public void UpdatingAResourceThatIsNotInTheListReturnsNull()
    {
        _resource.Description = "Description";
        _resourceRepository.Add(_resource);
        Resource updateResource = new Resource()
        {
            Name = "UpdatedName",
            Description = "UpdatedDescription",
        };
        
        
        Assert.AreEqual(_resourceRepository.Update(updateResource), null);
    }
    
    [TestMethod]
    public void DeleteResourceFromList()
    {
        Resource addedResource = _resourceRepository.Add(_resource);
        
        Assert.AreEqual(1, _resourceRepository.FindAll().Count);
        
        _resourceRepository.Delete(addedResource.Name);
        
        Assert.AreEqual(0, _resourceRepository.FindAll().Count);
    }
}