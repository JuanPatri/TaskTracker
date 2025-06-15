using Task = Domain.Task;
using Domain;
using DTOs.ProjectDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enums;

namespace BackendTest.DomainTest;

[TestClass]
public class ProjectTest
{
    private Project _project;

    
    [TestInitialize]
    public void OnInitialize()
    {
        var adminUser = new User()
        {
            Name = "Admin",
            LastName = "User",
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
            Name = "Test Project",
            Description = "Test Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>
            {
                adminRole
            }
        };

        adminRole.Project = _project;
    }

[TestMethod]
    public void CreateProjectTest()
    {
        Assert.IsNotNull(_project);
    }
    
    [TestMethod]
    public void AddNameForProjectTest()
    {
        _project.Name = "Project1";
        Assert.AreEqual("Project1", _project.Name);
    }
    
    [TestMethod]
    public void PutNameNullReturnsAnExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _project.Name = null);
    }
    
    [TestMethod]
    public void AddDescriptionForProjectTest()
    {
        _project.Description = "Project1 Description";
        Assert.AreEqual("Project1 Description", _project.Description);
    }
    
    [TestMethod]
    public void PutDescriptionNullReturnsAnExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _project.Description = null);
    }
    
    [TestMethod]
    public void PutDescriptionWithMoreThan400CharsReturnsAnExceptionTest()
    {
        string longDescription = new string('a', 401);
        Assert.ThrowsException<ArgumentException>(() => _project.Description = longDescription);
    }
    
    [TestMethod]
    public void AddStartDateForProjectTest()
    {
        DateOnly startDate = DateOnly.FromDateTime(DateTime.Today);
        _project.StartDate = startDate;
        Assert.AreEqual<DateOnly>(startDate, _project.StartDate);
    }

    [TestMethod]
    public void StartDateInPastReturnsExceptionTest()
    {
        DateOnly pastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        Assert.ThrowsException<ArgumentException>(() => _project.StartDate = pastDate);
    }
    
    [TestMethod]
    public void SetIdForProjectTest()
    {
        _project.Id = 1;
        Assert.AreEqual(1, _project.Id);
    }

    [TestMethod]
    public void PutLisTasksForProjectTest()
    {
        List<Task> tasks = new List<Task>();

        _project.Tasks = tasks; 
        Assert.AreEqual(tasks, _project.Tasks);
    }
    
    [TestMethod]
    public void AddExclusiveResourcesToProject()
    {
        List<Resource> resources = new List<Resource>();
        _project.ExclusiveResources = resources;
        Assert.AreEqual(resources, _project.ExclusiveResources);
    }
    
    [TestMethod]
    public void SetCriticalPathForProjectTest()
    {
        List<Task> criticalPath = new List<Task>();
        _project.CriticalPath = criticalPath;
        Assert.AreEqual(criticalPath, _project.CriticalPath);
    }

    [TestMethod]
    public void SetProjectRoleListTest()
    {
        List<ProjectRole> projectRoles = new List<ProjectRole>();
        _project.ProjectRoles = projectRoles;
        Assert.AreEqual(projectRoles, _project.ProjectRoles);
    }
}