using Backend.Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ResourceRepositoryTest
{
    [TestMethod]
    public void CreateResourceRepository()
    { 
        ResourceRepository _resourceRepository = new ResourceRepository();
        Assert.IsNotNull(_resourceRepository);
    }
}