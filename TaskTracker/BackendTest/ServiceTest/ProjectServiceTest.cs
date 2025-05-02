using Backend.Repository;
using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class ProjectServiceTest
{
    private ProjectService _projectService;
    private ProjectRepository _projectRepository;

    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        _projectService = new ProjectService(_projectRepository);
    }
    
    [TestMethod]
    public void CreateProjectService()
    {
        Assert.IsNotNull(_projectService);
    }
}