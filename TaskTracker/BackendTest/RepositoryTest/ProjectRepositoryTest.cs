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
    public void AddProjectToListTest()
    {
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.Find(p => p.Name == "New Project"), _project);
    }
    
    [TestMethod]
    public void SearchForAllProjectInTheListTest()
    {
        Assert.AreEqual(_projectRepository.FindAll().Count, 1);
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.FindAll().Count, 2);
    }
    
    [TestMethod]
    public void UpdateExistingProjectUpdatesFieldsCorrectlyTest()
    {
        _project.Name = "Project1";
        _projectRepository.Add(_project);
        Project updateProject = new Project()
        {
            Name = "Project1",
            Description = "UpdatedDescription",
            StartDate = DateTime.Now.AddDays(2),
            FinishDate = DateTime.Now.AddYears(2),
            Administrator = new User()
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@test.com",
                Password = "Admin123@",
                Admin = true,
                BirthDate = new DateTime(1990, 1, 1)
            }
        };
        var result = _projectRepository.Update(updateProject);
        Assert.IsNotNull(result);
        Assert.AreEqual(updateProject.Description, result.Description);
        Assert.AreEqual(updateProject.StartDate.Date, result.StartDate.Date);
        Assert.AreEqual(updateProject.FinishDate.Date, result.FinishDate.Date);
        Assert.AreEqual(updateProject.Administrator.Email, result.Administrator.Email);
    }
    
    [TestMethod]
    public void UpdatingAProjectThatIsNotInTheListReturnsNullTest()
    {
        _project.Name = "Project1";
        _projectRepository.Add(_project);
        Project updateProject = new Project()
        {
            Name = "NonExistentProject",
            Description = "UpdatedDescription",
            StartDate = DateTime.Now.AddDays(2),
            FinishDate = DateTime.Now.AddYears(2),
            Administrator = new User()
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@test.com",
                Password = "Admin123@",
                Admin = true,
                BirthDate = new DateTime(1990, 1, 1)
            }
        };
        var result = _projectRepository.Update(updateProject);
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void DeleteProjectFromListTest()
    {
        _project.Name = "ProjectDelete";
        _projectRepository.Add(_project);
        _projectRepository.Delete("ProjectDelete");
        Assert.IsNull(_projectRepository.Find(p => p.Name == "ProjectDelete"));
    }
}