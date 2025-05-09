using Backend.DTOs.TaskDTOs;

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
        double durationTask = 0.5;
        
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
    public void SetDescriptionNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Description = null);
        Assert.AreEqual("The description cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void SetDurationNullReturnsAnException()
    {
        double durationTask = 0.4;

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Duration = durationTask);
        Assert.AreEqual("The duration must be at least 0.5 hours (30 minutes)", ex.Message);
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

    [TestMethod]
    public void SetFinishToStartDependencies()
    {
        List<Task> finishToStartDependencies = new List<Task>
        {
            new Task { Title = "Task 1" },
            new Task { Title = "Task 2" }
        };
        _task.FinishToStartDependencies = finishToStartDependencies;
        
        Assert.AreEqual(finishToStartDependencies, _task.FinishToStartDependencies);
    }

    [TestMethod]
    public void SetStartToStartDependencies ()
    {
        List<Task> startToStartDependencies = new List<Task>
        {
            new Task { Title = "Task 1" },
            new Task { Title = "Task 2" }
        };
        _task.StartToStartDependencies = startToStartDependencies;
        
        Assert.AreEqual(startToStartDependencies, _task.StartToStartDependencies); 
    }

    [TestMethod] 
    public void SetSlackForTaskTest()
    {
        double slack = 0.5;
        _task.Slack = slack;
        Assert.AreEqual(slack, _task.Slack);
    }
[TestMethod]
public void FromDtoShouldCreateTaskWithCorrectValues()
{
    // Arrange
    TaskDataDTO taskDto = new TaskDataDTO
    {
        Title = "Task 1",
        Description = "Description of Task 1",
        Duration = 0.5,
        Status = Status.Blocked,
        Dependencies = new List<string> { "Task 1", "Task 2" },
        Resources = new List<(int, string)> { (1, "Resource 1") },
        FinishToStartDependencies = new List<string> { "Task 3" },
        StartToStartDependencies = new List<string> { "Task 4" },
        Slack = 1.0
    };

    List<Task> finishToStartDependencies = new List<Task>
    {
        new Task { Title = "Task 3", Description = "Description of Task 3" }
    };

    List<Task> startToStartDependencies = new List<Task>
    {
        new Task { Title = "Task 4", Description = "Description of Task 4" }
    };

    List<(int, Resource)> resources = new List<(int, Resource)>
    {
        (1, new Resource { Name = "Resource 1" })
    };

    Task task = Task.FromDto(taskDto, resources, startToStartDependencies, finishToStartDependencies);
    
    Assert.AreEqual("Task 1", task.Title);
    Assert.AreEqual("Description of Task 1", task.Description);
    Assert.AreEqual(0.5, task.Duration);
    Assert.AreEqual(Status.Blocked, task.Status);

    Assert.IsNotNull(task.Resources);
    Assert.AreEqual(1, task.Resources.Count);
    Assert.AreEqual(resources[0].Item1, task.Resources[0].Item1);      
    Assert.AreEqual(resources[0].Item2.Name, task.Resources[0].Item2.Name); 

    Assert.IsNotNull(task.FinishToStartDependencies);
    Assert.AreEqual(1, task.FinishToStartDependencies.Count);
    Assert.AreEqual("Task 3", task.FinishToStartDependencies[0].Title);
    Assert.AreEqual("Description of Task 3", task.FinishToStartDependencies[0].Description);

    Assert.IsNotNull(task.StartToStartDependencies);
    Assert.AreEqual(1, task.StartToStartDependencies.Count);
    Assert.AreEqual("Task 4", task.StartToStartDependencies[0].Title);
    Assert.AreEqual("Description of Task 4", task.StartToStartDependencies[0].Description);

    Assert.AreEqual(1.0, task.Slack);
}
}