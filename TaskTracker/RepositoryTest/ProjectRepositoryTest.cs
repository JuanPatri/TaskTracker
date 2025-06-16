using BusinessLogicTest.Context;
using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using DTOs.ProjectDTOs;
using Enums;
using Service;

namespace RepositoryTest;

[TestClass]
public class ProjectRepositoryTest
{
    private ProjectRepository _projectRepository;
    private Project _project;
    private ProjectService _projectService; 
    private TaskRepository _taskRepository;
    private ResourceTypeRepository _resourceTypeRepository;
    private UserRepository _userRepository;
    private UserService _userService;
    private CriticalPathService _criticalPathService;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _projectRepository = new ProjectRepository(_sqlContext); 
        _taskRepository = new TaskRepository(_sqlContext);
        _resourceTypeRepository = new ResourceTypeRepository(_sqlContext);
        _userRepository = new UserRepository(_sqlContext);
        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRepository, _userRepository, _userService, _criticalPathService);
        
        var adminUser = new User()
        {
            Name = "Admin",
            LastName = "Admin", 
            Email = "admin@test.com",
            Password = "Admin123@",
            Admin = true,
            BirthDate = new DateTime(1990, 1, 1)
        };
        
        var adminRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectAdmin,
            User = adminUser
        };
        
        _project = new Project()
        {
            Id = 1,
            Name = "Project1",
            Description = "Description1",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
            {
                adminRole
            }
        };
        
        adminRole.Project = _project;
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
        var result = _projectRepository.Find(p => p.Id == _project.Id);
        Assert.AreEqual(_project, result);
    }

    [TestMethod]
    public void SearchForAllProjectInTheListTest()
    {
        Assert.AreEqual(0, _projectRepository.FindAll().Count);
        _projectRepository.Add(_project);
        Assert.AreEqual(1, _projectRepository.FindAll().Count);
    }

   
    [TestMethod]
    public void UpdateExistingProjectUpdatesFieldsCorrectlyTest()
    {
        Project addedProject = _projectRepository.Add(_project);
        int originalId = addedProject.Id;
        
        Assert.AreEqual("Project1", addedProject.Name);
        Assert.AreEqual(1, _projectRepository.FindAll().Count);
        
        User updatedAdmin = new User
        {
            Name = "UpdatedAdmin",
            LastName = "UpdatedAdmin",
            Email = "updatedadmin@test.com",
            Password = "Admin123@",
            Admin = true,
            BirthDate = new DateTime(1990, 1, 1)
        };

        var updatedRole = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = updatedAdmin
        };


        var updatedProject = new Project
        {
            Id = originalId, 
            Name = "Updated Project",
            Description = "UpdatedDescription",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            ProjectRoles = new List<ProjectRole> { updatedRole }
        };
    
        updatedRole.Project = updatedProject;
        
        Project result = _projectRepository.Update(updatedProject);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Project", result.Name);
        Assert.AreEqual("UpdatedDescription", result.Description);
        
        var foundProject = _projectRepository.Find(p => p.Id == originalId);
        Assert.IsNotNull(foundProject);
        Assert.AreEqual("Updated Project", foundProject.Name);
    }

    [TestMethod]
    public void UpdatingAProjectThatIsNotInTheListReturnsNullTest()
    {
        _projectRepository.Add(_project);

        var nonExistentAdmin = new User()
        {
            Name = "NonExistent",
            LastName = "Admin",
            Email = "nonexistent@test.com",
            Password = "Admin123@",
            Admin = true,
            BirthDate = new DateTime(1990, 1, 1)
        };

        var nonExistentRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectAdmin,
            User = nonExistentAdmin
        };

        var nonExistentProjectDto = new ProjectDataDTO()
        {
            Id = 99,
            Name = "NonExistentProject",
            Description = "UpdatedDescription",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Users = new List<string> { "nonexistent@test.com" }
        };

        var updateProject = _projectService.FromDto(nonExistentProjectDto, new List<ProjectRole> { nonExistentRole });
        nonExistentRole.Project = updateProject;

        var result = _projectRepository.Update(updateProject);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void DeleteProjectFromListTest()
    {
        _projectRepository.Add(_project);
        Assert.AreEqual(1, _projectRepository.FindAll().Count);
        _projectRepository.Delete(_project.Id.ToString());
        Assert.AreEqual(0, _projectRepository.FindAll().Count);
    }
}