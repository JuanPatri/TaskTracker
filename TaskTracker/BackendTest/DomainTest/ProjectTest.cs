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
}