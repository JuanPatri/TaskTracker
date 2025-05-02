namespace BackendTest.DomainTest;
using Backend.Domain;
using Backend.Domain.Enums;

[TestClass]
public class ProjectTaskTest
{
    private ProjectTask _projectTask;

    [TestInitialize]
    public void OnInitialize()
    {
        _projectTask = new ProjectTask();
    }

    [TestMethod]
    public void CreateTaskTest()
    {
        Assert.IsNotNull(_projectTask);
    }

    [TestMethod]
    public void SetValidTitleUpdatesValueCorrectly()
    {
        string expectedTitle = "Valid Title";
        _projectTask.Title = expectedTitle;
        Assert.AreEqual(expectedTitle, _projectTask.Title);
    }
    
    [TestMethod]
    public void PutTitleNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _projectTask.Title = null);
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void PutTitleWhitespaceReturnAnExceptionTest()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _projectTask.Title = " ");
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateDescrptionForTaskTest()
    {
        _projectTask.Description = "Description";
        Assert.AreEqual("Description", _projectTask.Description);
    }

    [TestMethod]
    public void CreateDateForTaskTest()
    {
        DateOnly testDate = new DateOnly(2025, 4, 22);
        _projectTask.Date = testDate;

        Assert.AreEqual(testDate, _projectTask.Date);
    }

    [TestMethod]
    public void SetDateWithFutureDateThrowsArgumentException()
    {
        DateOnly futureDate = new DateOnly(2026, 4, 22);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _projectTask.Date = futureDate);
        Assert.AreEqual("The date cannot be in the future.", ex.Message);
    }

    [TestMethod]
    public void SetDurationForTaskTest()
    {
        TimeSpan durationTask = new TimeSpan(1, 5, 30, 0);
        
        _projectTask.DurationTask = durationTask;

        Assert.AreEqual(durationTask, _projectTask.DurationTask);
    }

    [TestMethod]
    public void SetStatusForTaskTest()
    {
        _projectTask.Status = Status.Pending;
        Assert.AreEqual(Status.Pending, _projectTask.Status);
    }
    
    [TestMethod]
    public void SetProjectForTaskTest()
    {
        Project project = new Project();
        _projectTask.Project = project;
        Assert.AreEqual(project, _projectTask.Project);
    }

    [TestMethod]
    public void SetProjectNullReturnsAnException()
    {
        Project project = null;
    
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _projectTask.Project = project);
        Assert.AreEqual("Project cannot be null", ex.Message);
    }

    [TestMethod]
    public void SetDependencyTaskTest()
    {
        List<ProjectTask> dependencyTask = new List<ProjectTask>();
    
        _projectTask.Dependencies = dependencyTask;
    
        Assert.AreEqual(dependencyTask, _projectTask.Dependencies);
    }
}