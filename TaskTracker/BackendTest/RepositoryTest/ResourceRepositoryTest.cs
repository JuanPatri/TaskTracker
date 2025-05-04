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
}