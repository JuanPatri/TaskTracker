using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ResourceRepositoryTest
{
    private ResourceRepository _resourceRepository;
    private Resource _resource;
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceRepository = new ResourceRepository();
        _resource = new Resource();
        _resource.Name = "new resource";
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
        Assert.AreEqual(_resourceRepository.Find(r => r.Name == "new resource"), _resource);
    }
    
    [TestMethod]
    public void SearchForAllResourceInTheList()
    {
        Assert.AreEqual(_resourceRepository.FindAll().Count, 0);
        _resourceRepository.Add(_resource);
        Assert.AreEqual(_resourceRepository.FindAll().Count, 1);
    }
    
    [TestMethod]
    public void UpdateExistingResourceUpdatesFieldsCorrectly()
    {
        _resource.Description = "Description";
        _resourceRepository.Add(_resource);
        Assert.AreEqual(_resource.Description, "Description");
        
        Resource updateResource = new Resource()
        {
            Name = "new resource",
            Description = "UpdatedDescription",
            Type = new ResourceType()
            {
                Name = "UpdatedType"
            }
        };
        
        _resourceRepository.Update(updateResource);
        Assert.AreEqual(_resource.Description, "UpdatedDescription");
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
        _resourceRepository.Add(_resource);
        Assert.AreEqual(_resourceRepository.FindAll().Count, 1);
        _resourceRepository.Delete(_resource.Name);
        Assert.AreEqual(_resourceRepository.FindAll().Count, 0);
    }
}