using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task = Domain.Task;

namespace DomainTest;

[TestClass]
public class TaskDependencyTest
{
    private TaskDependency _taskDependency;

    [TestInitialize]
    public void OnInitialize()
    {
        _taskDependency = new TaskDependency();
    }
    
    [TestMethod]
    public void TaskDependencyIsNotNull()
    {
        Assert.IsNotNull(_taskDependency);
    }
    
    [TestMethod]
    public void SetValidIdForTaskDependency()
    {
        _taskDependency.Id = 1;
        Assert.AreEqual(1, _taskDependency.Id);
    }
    
    [TestMethod]
    public void SetValidTaskForTaskDependency()
    {
        Task task = new Task { Title = "Task 1", Description = "Description 1" };
        _taskDependency.Task = task;
        Assert.AreEqual(task, _taskDependency.Task);
    }
    
    [TestMethod]
    public void SetNullTaskThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() => _taskDependency.Task = null);
        Assert.AreEqual("Task cannot be null", ex.Message);
    }
    
    [TestMethod]
    public void SetValidDependencyForTaskDependency()
    {
        Task dependency = new Task { Title = "Dependency Task", Description = "Dependency Description" };
        _taskDependency.Dependency = dependency;
        Assert.AreEqual(dependency, _taskDependency.Dependency);
    }
    
    [TestMethod]
    public void SetNullDependencyThrowsException()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() => _taskDependency.Dependency = null);
        Assert.AreEqual("Dependency cannot be null", ex.Message);
    }
}