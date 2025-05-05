using Backend.Domain;
using Backend.DTOs.ProjectDTOs;
using Task = Backend.Domain.Task;
using Backend.DTOs.UserDTOs;

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
        DateTime startDate = DateTime.Now.AddDays(1);
        _project.StartDate = startDate;
        Assert.AreEqual(startDate, _project.StartDate);
    }

    [TestMethod]
    public void StartDateInPastReturnsExceptionTest()
    {
        DateTime pastDate = DateTime.Now.AddDays(-1);
        Assert.ThrowsException<ArgumentException>(() => _project.StartDate = pastDate);
    }

    [TestMethod]
    public void AddFinishDateForProjectTest()
    {
        DateTime finishDate = DateTime.Now.AddDays(5);
        _project.FinishDate = finishDate;
        Assert.AreEqual(finishDate, _project.FinishDate);
    }
    
    [TestMethod]
    public void FinishDateBeforeStartDateReturnsExceptionTest()
    {
        var startDate = DateTime.Now.AddDays(5);
        var finishDate = DateTime.Now.AddDays(3);
        _project.StartDate = startDate;
        Assert.ThrowsException<ArgumentException>(() => _project.FinishDate = finishDate);
    }
    
    [TestMethod]
    public void AddAdministratorForProjectTest()
    {
        var admin = new User();
        _project.Administrator = admin;
        Assert.AreEqual(admin, _project.Administrator);
    }
    
    [TestMethod]
    public void PutAdministratorNullReturnsExceptionTest()
    {
        Assert.ThrowsException<ArgumentException>(() => _project.Administrator = null);
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
            StartDate = new DateTime(2026, 1, 1),
            FinishDate = new DateTime(2026, 12, 31),
            Administrator = new UserDataDTO
            {
                Name = "Pedro",
                LastName = "Rodriguez",
                Email = "prodriguez@gmail.com",
                BirthDate = new DateTime(2003, 03, 14),
                Password = "Pedro1234@",
            }
        };

        Project result = Project.FromDto(dto);
        
        Assert.AreEqual(dto.Id, result.Id);
        Assert.AreEqual(dto.Name, result.Name);
        Assert.AreEqual(dto.Description, result.Description);
        Assert.AreEqual(dto.StartDate, result.StartDate);
        Assert.AreEqual(dto.FinishDate, result.FinishDate);
        Assert.IsNotNull(result.Administrator);
        Assert.AreEqual(dto.Administrator.Name, result.Administrator.Name);
        Assert.AreEqual(dto.Administrator.LastName, result.Administrator.LastName);
        Assert.AreEqual(dto.Administrator.Email, result.Administrator.Email);
    }

}