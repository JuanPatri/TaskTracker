using Backend.Domain;
using Backend.Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ProjectRepositoryTest
{
    private ProjectRepository _projectRepository;
    private Project _project;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        _project = new Project();
        _project.Name = "New Project";
    }
    
    [TestMethod]
    public void CreateProjectRepositoryTest()
    {
        _projectRepository = new ProjectRepository();
        Assert.IsNotNull(_projectRepository);
    }
    
    [TestMethod]
    public void AddProjectToList()
    {
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.Find(p => p.Name == "New Project"), _project);
    }
    
    
}