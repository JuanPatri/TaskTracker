using Backend.Domain;
using Backend.Repository;
using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class ProjectServiceTest
{
    private ProjectService _projectService;
    private ProjectRepository _projectRepository;
    private Project _project;

    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        _projectService = new ProjectService(_projectRepository);

        _project = new Project()
        {
            Name = "Project 1",
        };
        _projectRepository.Add(_project);
    }
    
    [TestMethod]
    public void CreateProjectService()
    {
        Assert.IsNotNull(_projectService);
    }

    [TestMethod]
    public void AddProjectShouldReturnProject()
    {
        Project project = new Project()
        {
            Name = "Project 2",
            Description = "Description of project 2",
            StartDate = DateTime.Now,
            FinishDate = DateTime.Now.AddDays(10),
            Administrator = new User()
            {
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 01, 01),
                Password = "Pass123@",
                Admin = true
            }
        };
        Project? createdProject = _projectService.AddProject(project);
        Assert.IsNotNull(createdProject);
        Assert.AreEqual(_projectRepository.FindAll().Last(), createdProject);
    }
}