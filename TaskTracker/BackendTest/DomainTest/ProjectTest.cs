using Backend.Domain;
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
    public void IPutNameNullReturnsAnExceptionTest()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _project.Name = null);
        Assert.AreEqual("The project name cannot be empty", ex.Message);
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
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _project.Description = null);
        Assert.AreEqual("The project description cannot be empty", ex.Message);
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
}