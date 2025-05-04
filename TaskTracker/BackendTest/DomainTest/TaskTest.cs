namespace BackendTest.DomainTest;
using Backend.Domain;
using Backend.Domain.Enums;

[TestClass]
public class TaskTest
{
    private Task _task;

    [TestInitialize]
    public void OnInitialize()
    {
        _task = new Task();
    }

    [TestMethod]
    public void CreateTaskTest()
    {
        Assert.IsNotNull(_task);
    }

    [TestMethod]
    public void SetValidTitleUpdatesValueCorrectly()
    {
        string expectedTitle = "Valid Title";
        _task.Title = expectedTitle;
        Assert.AreEqual(expectedTitle, _task.Title);
    }
    
    [TestMethod]
    public void PutTitleNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Title = null);
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void PutTitleWhitespaceReturnAnExceptionTest()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Title = " ");
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateDescrptionForTaskTest()
    {
        _task.Description = "Description";
        Assert.AreEqual("Description", _task.Description);
    }

    [TestMethod]
    public void SetDurationForTaskTest()
    {
        TimeSpan durationTask = new TimeSpan(1, 5, 30, 0);
        
        _task.Duration = durationTask;

        Assert.AreEqual(durationTask, _task.Duration);
    }

    [TestMethod]
    public void SetStatusForTaskTest()
    {
        _task.Status = Status.Pending;
        Assert.AreEqual(Status.Pending, _task.Status);
    }
    
    [TestMethod]
    public void SetProjectForTaskTest()
    {
        Project project = new Project();
        _task.Project = project;
        Assert.AreEqual(project, _task.Project);
    }

    [TestMethod]
    public void SetProjectNullReturnsAnException()
    {
        Project project = null;
    
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Project = project);
        Assert.AreEqual("Project cannot be null", ex.Message);
    }

    [TestMethod]
    public void SetDependencyTaskTest()
    {
        List<Task> dependencyTask = new List<Task>();
    
        _task.Dependencies = dependencyTask;
    
        Assert.AreEqual(dependencyTask, _task.Dependencies);
    }

    [TestMethod]
    public void SetDescriptionNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Description = null);
        Assert.AreEqual("The description cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void SetDurationNullReturnsAnException()
    {
        TimeSpan durationTask = new TimeSpan(0, 0, 25, 0);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Duration = durationTask);
        Assert.AreEqual("The duration must be at least 30 minutes", ex.Message);
    }
    
    [TestMethod]
    public void SetResourcesTaskTest()
    {
        Resource res = new Resource();
        List<(int, Resource)> resources = new List<(int, Resource)>
        {
            (2, res)
        };
        
        _task.Resources = resources;
        Assert.AreEqual(resources, _task.Resources);
    }

}