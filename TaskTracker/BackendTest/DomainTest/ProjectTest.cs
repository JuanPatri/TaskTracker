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
}