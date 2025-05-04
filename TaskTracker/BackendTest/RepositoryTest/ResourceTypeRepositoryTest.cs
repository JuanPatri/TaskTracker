using Backend.Domain;
using Backend.Repository;

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
}