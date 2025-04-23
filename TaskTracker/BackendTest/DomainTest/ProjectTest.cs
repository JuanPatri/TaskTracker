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
    public void CreateProject()
    {
        Assert.IsNotNull(_project);
    }
    
    [TestMethod]
    public void CreateNameForProject()
    {
        _project.ValidateName = "Project1";
        Assert.AreEqual("Project1", _project.ValidateName);
    }
    
    [TestMethod]
    public void IPutNameNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _project.ValidateName = null);
        Assert.AreEqual("The project name cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void CreateDescriptionForProject()
    {
        _project.Description = "Project1 Description";
        Assert.AreEqual("Project1 Description", _project.Description);
    }
}