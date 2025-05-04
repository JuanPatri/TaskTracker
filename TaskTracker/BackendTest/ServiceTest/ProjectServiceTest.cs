using Backend.Domain;
using Backend.DTOs.ProjectDTOs;
using Backend.DTOs.UserDTOs;
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
        ProjectDataDTO project = new ProjectDataDTO()
        {
            Name = "Project 2",
            Description = "Description of project 2",
            StartDate = DateTime.Now.AddDays(1),
            FinishDate = DateTime.Now.AddDays(10),
            Administrator = new UserDataDTO()
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
        Assert.AreEqual(project.Name, createdProject.Name);
        Assert.AreEqual(project.Administrator.Email, createdProject.Administrator.Email);
        Assert.AreEqual(_projectRepository.FindAll().Last(), createdProject);
    }
    
    [TestMethod]
    public void RemoveProjectShouldRemoveProject()
    {
        Assert.AreEqual(_projectRepository.FindAll().Last(), _project);   

        GetProjectDTO projectToDelete = new GetProjectDTO()
        {
            Name = "Project 1",
        };
        _projectService.RemoveProject(projectToDelete);
        Assert.AreNotEqual(_projectRepository.FindAll().Last(), _project);
    }
    
    [TestMethod]
    public void GetProjectReturnProject()
    {
        GetProjectDTO projectToFind = new GetProjectDTO()
        {
            Name = "Project 1",
        };
        Assert.AreEqual(_projectService.GetProject(projectToFind), _project);
    }
    
    [TestMethod]
    public void GetAllProjectsReturnAllProjects()
    {
        List<Project> projects = _projectService.GetAllProjects();
        Assert.IsNotNull(projects);
        Assert.AreEqual(_projectRepository.FindAll().Count(), projects.Count);
    }

    [TestMethod]
    public void UpdateProjectShouldReturnUpdatedProject()
    {
        ProjectDataDTO project = new ProjectDataDTO()
        {
            Name = "Project 1",
            Description = "Updated description",
            StartDate = DateTime.Now.AddDays(2),
            FinishDate = DateTime.Now.AddDays(12),
            Administrator = new UserDataDTO()
            {
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 01, 01),
                Password = "Pass123@",
                Admin = true
            }
        };
        _projectService.UpdateProject(project);
        Assert.AreEqual(_project.Name, project.Name);
    }
}