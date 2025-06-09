using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Task = Domain.Task;
using DTOs.ProjectDTOs;
using Enums;

namespace RepositoryTest;

[TestClass]
public class ProjectRepositoryTest
{
    private ProjectRepository _projectRepository;
    private Project _project;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        
        User adminUser = new User()
        {
            Name = "Admin",
            LastName = "Admin", 
            Email = "admin@test.com",
            Password = "Admin123@",
            Admin = true,
            BirthDate = new DateTime(1990, 1, 1)
        };
        
        ProjectRole adminRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectAdmin,
            User = adminUser
        };
        
        ProjectDataDTO projectDto = new ProjectDataDTO()
        {
            Id = 1,
            Name = "Project1",
            Description = "Description1",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Users = new List<string> { "admin@test.com" }
        };
        
        _project = Project.FromDto(projectDto, new List<ProjectRole> { adminRole });
        
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
        
        User updatedAdmin = new User()
        {
            Name = "UpdatedAdmin",
            LastName = "UpdatedAdmin",
            Email = "updatedadmin@test.com",
            Password = "Admin123@",
            Admin = true,
            BirthDate = new DateTime(1990, 1, 1)
        };
        
        ProjectRole updatedRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectAdmin,
            User = updatedAdmin
        };
        
        ProjectDataDTO updatedProjectDto = new ProjectDataDTO()
        {
            Id = 2,
            Name = "Updated Project",
            Description = "UpdatedDescription",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Users = new List<string> { "updatedadmin@test.com" }
        };
        
        Project updateProject = Project.FromDto(updatedProjectDto, new List<ProjectRole> { updatedRole });
        updatedRole.Project = updateProject;
        
        _projectRepository.Update(updateProject);
        Project updatedProjectFromRepo = _projectRepository.Find(p => p.Id == 2);
        
        Assert.IsNotNull(updatedProjectFromRepo);
        Assert.AreEqual(updateProject.Name, updatedProjectFromRepo.Name);
        Assert.AreEqual(updateProject.Description, updatedProjectFromRepo.Description);
        Assert.AreEqual(updateProject.StartDate, updatedProjectFromRepo.StartDate);
        
        ProjectRole adminRole = updatedProjectFromRepo.ProjectRoles.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectAdmin);
        Assert.IsNotNull(adminRole);
        Assert.AreEqual("updatedadmin@test.com", adminRole.User.Email);
    }
    
    [TestMethod]
    public void UpdatingAProjectThatIsNotInTheListReturnsNullTest()
    {
        _projectRepository.Add(_project);
        
        User nonExistentAdmin = new User()
        {
            Name = "NonExistent",
            LastName = "Admin",
            Email = "nonexistent@test.com",
            Password = "Admin123@",
            Admin = true,
            BirthDate = new DateTime(1990, 1, 1)
        };
        
        ProjectRole nonExistentRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectAdmin,
            User = nonExistentAdmin
        };
        
        ProjectDataDTO nonExistentProjectDto = new ProjectDataDTO()
        {
            Id = 99,
            Name = "NonExistentProject",
            Description = "UpdatedDescription",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Users = new List<string> { "nonexistent@test.com" }
        };
        
        Project updateProject = Project.FromDto(nonExistentProjectDto, new List<ProjectRole> { nonExistentRole });
        nonExistentRole.Project = updateProject;
        
        Project result = _projectRepository.Update(updateProject);
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