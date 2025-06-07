using Task = Domain.Task;
using Domain;
using DTOs.ProjectDTOs;
using DTOs.UserDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendTest.DomainTest;

[TestClass]
public class ProjectTest
{
    private Project _project;

    [TestInitialize]
    public void OnInitialize()
    {
        _project = new Project();
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
    public void SetIdNegativeReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _project.Id = -1);
    }

    [TestMethod]
    public void PutLisTasksForProjectTest()
    {
        List<Task> tasks = new List<Task>();

        _project.Tasks = tasks; 
        Assert.AreEqual(tasks, _project.Tasks);
    }
    
    
    [TestMethod]
    public void FromDtoShouldMapAllPropertiesCorrectly()
    {
        ProjectDataDTO dto = new ProjectDataDTO()
        {
            Id = 5,
            Name = "Test Project",
            Description = "This is a test project",
            StartDate = new DateOnly (2026, 1, 1),
            Administrator = new UserDataDTO
            {
                Name = "Pedro",
                LastName = "Rodriguez",
                Email = "prodriguez@gmail.com",
                BirthDate = new DateTime(2003, 03, 14),
                Password = "Pedro1234@",
            },
            Users = new List<string>
            {
                "test@gmail.com"
            }
                
        };

        List<User> users = new List<User>()
        {
            new User()
            {
                Email = "test@gmail.com"
            }
        };

        Project result = Project.FromDto(dto, users);
        
        Assert.AreEqual(dto.Id, result.Id);
        Assert.AreEqual(dto.Name, result.Name);
        Assert.AreEqual(dto.Description, result.Description);
        Assert.AreEqual(dto.StartDate, result.StartDate);
        Assert.IsNotNull(result.Administrator);
        Assert.AreEqual(dto.Administrator.Name, result.Administrator.Name);
        Assert.AreEqual(dto.Administrator.LastName, result.Administrator.LastName);
        Assert.AreEqual(dto.Administrator.Email, result.Administrator.Email);
    }

    [TestMethod]
    public void SetUserListForProjectTest()
    {
        List<User> users = new List<User>();
        _project.Users = users;
        Assert.AreEqual(users, _project.Users);
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

}