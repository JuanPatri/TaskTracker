using Backend.Domain;
using Backend.Repository;
using Task = Backend.Domain.Task;

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
        _project = new Project()
        {
            Id = 1,
            Name = "Project1",
            Description = "Description1",
            StartDate = DateTime.Now.AddDays(1),
            FinishDate = DateTime.Now.AddYears(1),
            Administrator = new User()
        };
    }
    
    [TestMethod]
    public void CreateProjectRepositoryTest()
    {
        Assert.IsNotNull(_projectRepository);
    }
    
    [TestMethod]
    public void AddProjectToListTest()
    {
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.Find(p => p.Id == 1), _project);
    }
    
    [TestMethod]
    public void SearchForAllProjectInTheListTest()
    {
        Assert.AreEqual(_projectRepository.FindAll().Count, 0);
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.FindAll().Count, 1);
    }
    
    [TestMethod]
    public void UpdateExistingProjectUpdatesFieldsCorrectlyTest()
    {
        _project.Id = 2;
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.FindAll().Count, 1);
        Project updateProject = new Project()
        {
            Id = 2,
            Name = "Updated Project",
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
        _projectRepository.Update(updateProject);
        var updatedProject = _projectRepository.Find(p => p.Id == 2);
        Assert.IsNotNull(updatedProject);
        Assert.AreEqual(updateProject.Name, updatedProject.Name);
        Assert.AreEqual(updateProject.Description, updatedProject.Description);
        Assert.AreEqual(updateProject.StartDate, updatedProject.StartDate);
        Assert.AreEqual(updateProject.FinishDate, updatedProject.FinishDate);
        Assert.AreEqual(updateProject.Administrator.Email, updatedProject.Administrator.Email);
    }
    
    [TestMethod]
    public void UpdatingAProjectThatIsNotInTheListReturnsNullTest()
    {
        _project.Name = "Project1";
        _projectRepository.Add(_project);
        Project updateProject = new Project()
        {
            Id = 99,
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
                BirthDate = new DateTime(1990, 1, 1),
            },
            Tasks = new List<Task>(),
            Users = new List<User>()
        };
        
        var result = _projectRepository.Update(updateProject);
        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void DeleteProjectFromListTest()
    {
        _projectRepository.Add(_project);
        Assert.AreEqual(_projectRepository.FindAll().Count, 1);
        _projectRepository.Delete(_project.Id.ToString());
        Assert.AreEqual(_projectRepository.FindAll().Count, 0);
    }
}