using Backend.Domain;
using Backend.Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ResourceRepositoryTest
{
    private ResourceRepository _resourceRepository;
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceRepository = new ResourceRepository();
    }
    [TestMethod]
    public void CreateResourceRepository()
    { 
        Assert.IsNotNull(_resourceRepository);
    }
    
    [TestMethod]
    public void AddResourceToRepository()
    {
        Resource resource = new Resource();
        resource.Name = "new resource";
        _resourceRepository.Add(resource);
        Assert.AreEqual(_resourceRepository.Find(r => r.Name == "new resource"), resource);
    }
}