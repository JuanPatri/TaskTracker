namespace BackendTest.RepositoryTest;

[TestClass]
public class ProjectRepositoryTest
{
    private ProjectRepository _projectRepository;
    
    [TestMethod]
    public void CreateProjectRepository()
    {
        _projectRepository = new ProjectRepository();
        Assert.IsNotNull(_projectRepository);
    }
}